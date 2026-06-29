using HRMS.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Data.Seeds
{
    public static class OnboardingStatusMasterSeed
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OnboardingStatusMaster>().HasData(
                new OnboardingStatusMaster { Id = Guid.Parse("D0100000-0000-0000-0000-000000000001"), Name = "Not Onboarded", DisplayOrder = 1, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new OnboardingStatusMaster { Id = Guid.Parse("D0100000-0000-0000-0000-000000000002"), Name = "In Progress", DisplayOrder = 2, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new OnboardingStatusMaster { Id = Guid.Parse("D0100000-0000-0000-0000-000000000003"), Name = "Onboarded", DisplayOrder = 3, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new OnboardingStatusMaster { Id = Guid.Parse("D0100000-0000-0000-0000-000000000004"), Name = "Exited", DisplayOrder = 4, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );
        }
    }
}
