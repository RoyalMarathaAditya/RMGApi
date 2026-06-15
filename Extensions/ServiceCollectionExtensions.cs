using HRMS.Api.Data;
using HRMS.Api.Models;
using HRMS.Api.Profiles;
using HRMS.Api.Repositories;
using HRMS.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HRMS.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHrmsServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.Configure<JwtSettings>(configuration.GetSection("Jwt"));

            // AutoMapper
            services.AddAutoMapper(typeof(MappingProfile));

            // Repositories and services
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<ISkillRepository, SkillRepository>();
            services.AddScoped<IProjectDetailsRepository, ProjectDetailsRepository>();

            // Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}
