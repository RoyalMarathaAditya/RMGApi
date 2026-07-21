using FluentValidation;
using FluentValidation.AspNetCore;
using HRMS.Api.Data;
using HRMS.Api.Models;
using HRMS.Api.Profiles;
using HRMS.Api.Repositories;
using HRMS.Api.Repositories.Interfaces;
using HRMS.Api.Repositories.Interfaces.RMG;
using HRMS.Api.Repositories.RMG;
using HRMS.Api.Repositories.UserManagement;
using HRMS.Api.Services;
using HRMS.Api.Services.Interfaces;
using HRMS.Api.Services.Interfaces.RMG;
using HRMS.Api.Services.Interfaces.UserManagement;
using HRMS.Api.Services.RMG;
using HRMS.Api.Services.UserManagement;
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
            services.AddScoped<IClientService, ClientService>();
            services.AddScoped<ISkillRepository, SkillRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<ILeaveRepository, LeaveRepository>();
            services.AddScoped<IPipRepository, PipRepository>();
            services.AddScoped<IDesignationRepository, DesignationRepository>();
            services.AddScoped<IProjectStatusRepository, ProjectStatusRepository>();
            services.AddScoped<IProbableNextAssignmentRepository, ProbableNextAssignmentRepository>();
            services.AddScoped<IBillableDateProbabilityRepository, BillableDateProbabilityRepository>();
            services.AddScoped<ICurrentBillingStatusRepository, CurrentBillingStatusRepository>();
            services.AddScoped<IBillingBucketRepository, BillingBucketRepository>();
            services.AddScoped<IOnboardingStatusRepository, OnboardingStatusRepository>();
            services.AddScoped<IAgeingBucketRepository, AgeingBucketRepository>();
            services.AddScoped<IColumnMappingRepository, ColumnMappingRepository>();
            services.AddScoped<IColumnValueMappingRepository, ColumnValueMappingRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ILeaveService, LeaveService>();
            services.AddScoped<IPipService, PipService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IBulkImportService, BulkImportService>();
            services.AddScoped<DynamicExcelMapper>();
            services.AddScoped<IDesignationService, DesignationService>();
            services.AddScoped<IProjectStatusService, ProjectStatusService>();
            services.AddScoped<IProbableNextAssignmentService, ProbableNextAssignmentService>();
            services.AddScoped<IBillableDateProbabilityService, BillableDateProbabilityService>();
            services.AddScoped<ICurrentBillingStatusService, CurrentBillingStatusService>();
            services.AddScoped<IBillingBucketService, BillingBucketService>();
            services.AddScoped<IOnboardingStatusService, OnboardingStatusService>();
            services.AddScoped<IAgeingBucketService, AgeingBucketService>();
            services.AddScoped<IColumnMappingService, ColumnMappingService>();
            services.AddScoped<IColumnValueMappingService, ColumnValueMappingService>();
            services.AddScoped<IExcelExportService, ExcelExportService>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserManagementService, UserManagementService>();
            services.AddScoped<IRoleManagementService, RoleManagementService>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IUserSynchronizationService, UserSynchronizationService>();

            services.AddScoped<IResourceAllocationRepository, ResourceAllocationRepository>();
            services.AddScoped<IResourceAllocationHistoryRepository, ResourceAllocationHistoryRepository>();
            services.AddScoped<IResourceRequestRepository, ResourceRequestRepository>();

            services.AddScoped<IResourceAllocationService, ResourceAllocationService>();
            services.AddScoped<IResourceRequestService, ResourceRequestService>();
            services.AddScoped<IRmgDashboardService, RmgDashboardService>();
            services.AddScoped<IReportService, ReportService>();

            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
            });

            services.AddHealthChecks();

            return services;
        }
    }
}
