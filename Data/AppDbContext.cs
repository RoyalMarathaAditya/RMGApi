using HRMS.Api.Configurations;
using HRMS.Api.Data.Seeds;
using HRMS.Api.Models;
using HRMS.Api.Models.RMG;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeSkill> EmployeeSkills { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<EmployeeLeave> EmployeeLeaves { get; set; }
        public DbSet<PIP> PIPs { get; set; }

        public DbSet<RoleMaster> RoleMasters { get; set; }
        public DbSet<EmploymentTypeMaster> EmploymentTypeMasters { get; set; }
        public DbSet<StatusMaster> StatusMasters { get; set; }
        public DbSet<WorkModelMaster> WorkModelMasters { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Practice> Practices { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<LeaveTypeMaster> LeaveTypeMasters { get; set; }
        public DbSet<PricingTypeMaster> PricingTypeMasters { get; set; }
        public DbSet<ProjectTypeMaster> ProjectTypeMasters { get; set; }
        public DbSet<DepartmentTypeMaster> DepartmentTypeMasters { get; set; }
        public DbSet<DesignationMaster> DesignationMasters { get; set; }
        public DbSet<OnboardingTypeMaster> OnboardingTypeMasters { get; set; }
        public DbSet<ImportAudit> ImportAudits { get; set; }
        public DbSet<SubPracticeMaster> SubPracticeMasters { get; set; }

        public DbSet<ResourceAllocation> ResourceAllocations { get; set; }
        public DbSet<ResourceAllocationHistory> ResourceAllocationHistory { get; set; }
        public DbSet<ResourceRequest> ResourceRequests { get; set; }
        public DbSet<EmployeeImportHistory> EmployeeImportHistory { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedOn = DateTime.UtcNow;
                        break;
                    case EntityState.Modified:
                        entry.Entity.ModifiedOn = DateTime.UtcNow;
                        break;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            modelBuilder.ApplyMasterSeeds();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
        }
    }
}
