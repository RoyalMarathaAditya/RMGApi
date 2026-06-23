using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Data.Seeds
{
    public static class MasterSeedExtension
    {
        public static void ApplyMasterSeeds(this ModelBuilder modelBuilder)
        {
            RoleMasterSeed.Seed(modelBuilder);
            EmploymentTypeMasterSeed.Seed(modelBuilder);
            StatusMasterSeed.Seed(modelBuilder);
            WorkModelMasterSeed.Seed(modelBuilder);
            LocationMasterSeed.Seed(modelBuilder);
            PracticeMasterSeed.Seed(modelBuilder);
            SkillMasterSeed.Seed(modelBuilder);
            LeaveTypeMasterSeed.Seed(modelBuilder);
            PricingTypeMasterSeed.Seed(modelBuilder);
            ProjectTypeMasterSeed.Seed(modelBuilder);
            AllocationStatusMasterSeed.Seed(modelBuilder);
            EmployeeProjectStatusMasterSeed.Seed(modelBuilder);
            DepartmentTypeMasterSeed.Seed(modelBuilder);
            DesignationMasterSeed.Seed(modelBuilder);
        }
    }
}
