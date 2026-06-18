using HRMS.Api.Data;
using HRMS.Api.Extensions;
using HRMS.Api.Middleware;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Serilog;
using System.Collections.Generic;

Console.Title = "RMG.Api";

Console.WriteLine("==================================");
Console.WriteLine("     RMG API Started Successfully");
Console.WriteLine("==================================");

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog early
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
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

// Configure Swashbuckle v10 for Bearer Input
builder.Services.AddSwaggerGen(options =>
{
    // 1. Define Bearer Scheme (MUST use lowercase "bearer" for both ID and Scheme)
    options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT Bearer token"
    });

    // 2. Wrap the collection initializer inside the required document lambda delegate
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("bearer", document),
            new List<string>()
        }
    });
});

var app = builder.Build();

app.UseSerilogRequestLogging();

// FIX 1: Crucial Networking and Client Security must happen at the absolute top
app.UseHttpsRedirection();
app.UseStaticFiles(); // Serves files from wwwroot (required for swagger-ui-init.js)
app.UseCors("DefaultCors");

// FIX 2: Expose Swagger assets BEFORE custom middlewares execute 
// so your authentication path checks never intercept swagger asset engines
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "RMG API V1");
    c.InjectJavascript("/swagger-ui-init.js");
});

// FIX 3: Discover endpoints and action attributes before running custom authorization
app.UseRouting();

// Global Diagnostic Middlewares
app.UseMiddleware<LoggerMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();

// Custom Security Middlewares (Order is vital!)
app.UseMiddleware<HRMS.Api.Middleware.AuthenticationMiddleware>(); // 1st: Populates context.User
app.UseMiddleware<CustomAuthorizationMiddleware>();                // 2nd: Checks permissions

// Setup basic endpoint routing maps
app.MapControllers();
app.MapGet("/", () => Results.Redirect("/swagger"));

//// Automatically handle database migrations at startup
//using (var scope = app.Services.CreateScope())
//{
//    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//    await dbContext.Database.MigrateAsync();
//}


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

        throw; // IMPORTANT: fail fast so issue is visible
    }
}



app.Run();
