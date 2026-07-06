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
            DepartmentTypeMasterSeed.Seed(modelBuilder);
            DesignationMasterSeed.Seed(modelBuilder);
            ProjectStatusMasterSeed.Seed(modelBuilder);
            ProbableNextAssignmentMasterSeed.Seed(modelBuilder);
            BillableDateProbabilityMasterSeed.Seed(modelBuilder);
            CurrentBillingStatusMasterSeed.Seed(modelBuilder);
            BillingBucketMasterSeed.Seed(modelBuilder);
            OnboardingStatusMasterSeed.Seed(modelBuilder);
            AgeingBucketMasterSeed.Seed(modelBuilder);
            ColumnMappingSeed.Seed(modelBuilder);
            ColumnValueMappingSeed.Seed(modelBuilder);
            OnboardingTypeMasterSeed.Seed(modelBuilder);
            CSMRevenueTypeMasterSeed.Seed(modelBuilder);
            ClientMasterSeed.Seed(modelBuilder);
            ProjectMasterSeed.Seed(modelBuilder);
        }
    }
}
