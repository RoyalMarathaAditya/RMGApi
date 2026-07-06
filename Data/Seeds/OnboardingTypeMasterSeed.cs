using HRMS.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Data.Seeds
{
    public static class OnboardingTypeMasterSeed
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OnboardingTypeMaster>().HasData(
                new OnboardingTypeMaster { Id = Guid.Parse("D0200000-0000-0000-0000-000000000001"), Name = "Direct Join", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new OnboardingTypeMaster { Id = Guid.Parse("D0200000-0000-0000-0000-000000000002"), Name = "Internal Transfer", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new OnboardingTypeMaster { Id = Guid.Parse("D0200000-0000-0000-0000-000000000003"), Name = "Academy Hire", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new OnboardingTypeMaster { Id = Guid.Parse("D0200000-0000-0000-0000-000000000004"), Name = "Lateral Hire", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new OnboardingTypeMaster { Id = Guid.Parse("D0200000-0000-0000-0000-000000000005"), Name = "Contract to Hire", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );
        }
    }
}
