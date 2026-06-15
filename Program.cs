using System.Text;
using HRMS.Api.Data;
using HRMS.Api.Extensions;
using HRMS.Api.Middleware;
using HRMS.Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;

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

var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()!;
var key = Encoding.UTF8.GetBytes(jwtSettings.Key);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(key),
        };

        options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
        {
            OnAuthenticationFailed = ctx =>
            {
                var logger = ctx.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogError(ctx.Exception, "JWT Authentication failed.");
                return Task.CompletedTask;
            },
            OnMessageReceived = ctx =>
            {
                var logger = ctx.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                if (!ctx.Request.Headers.ContainsKey("Authorization"))
                {
                    logger.LogInformation("No Authorization header present on request {Path}", ctx.Request.Path);
                }
                else
                {
                    logger.LogInformation("Authorization header present for request {Path}", ctx.Request.Path);
                }
                return Task.CompletedTask;
            },
            OnTokenValidated = ctx =>
            {
                var logger = ctx.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                var sub = ctx.Principal?.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
                logger.LogInformation("JWT token validated for subject {Sub}", sub);
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
// Required for Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSerilogRequestLogging();

// Request/response logging
app.UseMiddleware<LoggerMiddleware>();

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseCors("DefaultCors");
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();
app.MapGet("/", () => Results.Redirect("/swagger"));

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<HrmsDbContext>();

    // Apply pending migrations automatically
    await dbContext.Database.MigrateAsync();
    //await DbInitializer.SeedAsync(dbContext);
}

app.Run();
