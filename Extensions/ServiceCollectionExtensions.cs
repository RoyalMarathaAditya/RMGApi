using HRMS.Api.Data;
using HRMS.Api.Models;
using HRMS.Api.Profiles;
using HRMS.Api.Repositories;
using HRMS.Api.Repositories.Interfaces;
using HRMS.Api.Services;
using HRMS.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

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
            //services.AddAutoMapper(typeof(MappingProfile));
            services.AddAutoMapper(cfg => { }, typeof(MappingProfile).Assembly);

            // Repositories and services
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<ISkillRepository, SkillRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IDesignationRepository, DesignationRepository>();
            services.AddScoped<IEmployeeHierarchyRepository, EmployeeHierarchyRepository>();


            // Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IDesignationService, DesignationService>();
            services.AddScoped<IEmployeeHierarchyService, EmployeeHierarchyService>();


            return services;
        }
    }
}
