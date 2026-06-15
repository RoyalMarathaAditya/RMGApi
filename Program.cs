using HRMS.Api.Data;
using HRMS.Api.Extensions;
using HRMS.Api.Middleware;
using Microsoft.EntityFrameworkCore;
using Serilog;

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
builder.Services.AddOpenApi();
// Required for Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSerilogRequestLogging();

// Custom middleware
app.UseMiddleware<LoggerMiddleware>();

app.UseMiddleware<ExceptionMiddleware>();

app.UseMiddleware<AuthenticationMiddleware>();

app.UseMiddleware<AuthorizationMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseCors("DefaultCors");


app.MapControllers();
app.MapGet("/", () => Results.Redirect("/swagger"));

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Apply pending migrations automatically
    await dbContext.Database.MigrateAsync();
 
}

app.Run();
