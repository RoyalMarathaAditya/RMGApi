using System.Diagnostics;
using HRMS.Api.Data;
using HRMS.Api.Extensions;
using HRMS.Api.Middleware;
using Microsoft.EntityFrameworkCore;
using Serilog;

Console.Title = "RMG.Api";

Process? reactProcess = null;
var isDevelopment = string.Equals(
    Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
    "Development",
    StringComparison.OrdinalIgnoreCase);

if (isDevelopment && !args.Contains("--no-react"))
{
    var frontendDir = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "Frontend"));

    if (Directory.Exists(frontendDir))
    {
        Console.WriteLine("==========================================");
        Console.WriteLine("   HRMS Resource Management Startup");
        Console.WriteLine("==========================================");
        Console.WriteLine();
        Console.WriteLine("Starting Backend API...");
        Console.WriteLine();

        var nodeModulesPath = Path.Combine(frontendDir, "node_modules");
        if (!Directory.Exists(nodeModulesPath))
        {
            Console.Write("Running npm install...");
            var npmInstall = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/c npm install",
                    WorkingDirectory = frontendDir,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                }
            };
            npmInstall.Start();
            npmInstall.WaitForExit(120000);
            Console.WriteLine(" completed.");
            Console.WriteLine();
        }

        Console.WriteLine("Starting React development server...");
        Console.WriteLine();

        reactProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/k title React Dev Server && npm run dev",
                WorkingDirectory = frontendDir,
                UseShellExecute = true,
                CreateNoWindow = false,
                WindowStyle = ProcessWindowStyle.Normal,
            }
        };
        reactProcess.Start();

        Console.WriteLine("Waiting for React to become available...");
        Console.WriteLine();

        using var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(3) };
        var reactReady = false;
        for (var i = 0; i < 30; i++)
        {
            try
            {
                var response = await httpClient.GetAsync("http://localhost:5173");
                if (response.IsSuccessStatusCode)
                {
                    reactReady = true;
                    break;
                }
            }
            catch
            {
                // Not ready yet
            }
            await Task.Delay(2000);
        }

        if (reactReady)
        {
            Console.WriteLine("React Running:");
            Console.WriteLine("http://localhost:5173");
            Console.WriteLine();
        }
        else
        {
            Console.WriteLine("Warning: React dev server did not start within expected time.");
            Console.WriteLine("Continuing with backend startup...");
            Console.WriteLine();
        }

        if (reactReady && OperatingSystem.IsWindows())
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "http://localhost:5173",
                    UseShellExecute = true,
                });
            }
            catch
            {
                // Best effort browser launch
            }
        }

        Console.WriteLine("Backend Running:");
        Console.WriteLine("https://localhost:7132");
        Console.WriteLine();
        Console.WriteLine("Application Started Successfully.");
        Console.WriteLine("==========================================");
        Console.WriteLine();
    }
    else
    {
        Console.WriteLine($"Frontend directory not found: {frontendDir}");
        Console.WriteLine("Starting backend only.");
        Console.WriteLine();
    }
}

AppDomain.CurrentDomain.ProcessExit += (_, _) =>
{
    if (reactProcess is { HasExited: false })
    {
        try
        {
            reactProcess.Kill(entireProcessTree: true);
            reactProcess.WaitForExit(5000);
        }
        catch
        {
            // Best effort cleanup
        }
    }
};

Console.CancelKeyPress += (_, e) =>
{
    if (reactProcess is { HasExited: false })
    {
        try
        {
            reactProcess.Kill(entireProcessTree: true);
            reactProcess.WaitForExit(3000);
        }
        catch
        {
            // Best effort cleanup
        }
    }
    e.Cancel = false;
};

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/rmg-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddHrmsServices(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCors", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

var appLifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
appLifetime.ApplicationStopping.Register(() =>
{
    if (reactProcess is { HasExited: false })
    {
        try
        {
            reactProcess.Kill(entireProcessTree: true);
            reactProcess.WaitForExit(5000);
        }
        catch
        {
            // Best effort cleanup
        }
    }
});

app.UseSerilogRequestLogging();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseStaticFiles();
app.UseCors("DefaultCors");
app.UseResponseCompression();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "RMG API V1");
});

app.UseRouting();

app.UseMiddleware<LoggerMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<AuthenticationMiddleware>();
app.UseMiddleware<CustomAuthorizationMiddleware>();

app.MapControllers();
app.MapHealthChecks("/health");
app.MapGet("/", () => Results.Redirect("/swagger"));

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        logger.LogInformation("Applying database migrations...");
        await dbContext.Database.MigrateAsync();
        logger.LogInformation("Database migration completed successfully.");

        logger.LogInformation("Seeding initial data...");
        await HRMS.Api.Data.Seeders.DbSeeder.SeedAsync(dbContext);
        logger.LogInformation("Database seeding completed successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error occurred during migration or seeding.");
        throw;
    }
}

app.Run();
