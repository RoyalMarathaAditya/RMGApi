using FluentValidation;
using FluentValidation.AspNetCore;
using HRMS.Api.Data;
using HRMS.Api.Models;
using HRMS.Api.Profiles;
using HRMS.Api.Repositories;
using HRMS.Api.Repositories.Interfaces;
using HRMS.Api.Repositories.Interfaces.RMG;
using HRMS.Api.Repositories.RMG;
using HRMS.Api.Services;
using HRMS.Api.Services.Interfaces;
using HRMS.Api.Services.Interfaces.RMG;
using HRMS.Api.Services.RMG;
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

            services.AddAutoMapper(cfg => { }, typeof(MappingProfile).Assembly);

            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<MappingProfile>();

            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<ISkillRepository, SkillRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<ILeaveRepository, LeaveRepository>();
            services.AddScoped<IPipRepository, PipRepository>();
            services.AddScoped<IDesignationRepository, DesignationRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ILeaveService, LeaveService>();
            services.AddScoped<IPipService, PipService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IBulkImportService, BulkImportService>();
            services.AddScoped<IDesignationService, DesignationService>();

            services.AddScoped<IResourceAllocationRepository, ResourceAllocationRepository>();
            services.AddScoped<IResourceAllocationHistoryRepository, ResourceAllocationHistoryRepository>();
            services.AddScoped<IResourceRequestRepository, ResourceRequestRepository>();

            services.AddScoped<IResourceAllocationService, ResourceAllocationService>();
            services.AddScoped<IResourceRequestService, ResourceRequestService>();
            services.AddScoped<IRmgDashboardService, RmgDashboardService>();

            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
            });

            services.AddHealthChecks();

            return services;
        }
    }
}
