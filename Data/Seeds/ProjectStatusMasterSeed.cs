using HRMS.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Data.Seeds
{
    public static class ProjectStatusMasterSeed
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProjectStatusMaster>().HasData(
                new ProjectStatusMaster { Id = Guid.Parse("A0000000-0000-0000-0000-000000000001"), Name = "Utilized", DisplayOrder = 1, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new ProjectStatusMaster { Id = Guid.Parse("A0000000-0000-0000-0000-000000000002"), Name = "Billable", DisplayOrder = 2, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new ProjectStatusMaster { Id = Guid.Parse("A0000000-0000-0000-0000-000000000003"), Name = "Long Leave", DisplayOrder = 3, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new ProjectStatusMaster { Id = Guid.Parse("A0000000-0000-0000-0000-000000000004"), Name = "Management", DisplayOrder = 4, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new ProjectStatusMaster { Id = Guid.Parse("A0000000-0000-0000-0000-000000000005"), Name = "Onboarding", DisplayOrder = 5, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new ProjectStatusMaster { Id = Guid.Parse("A0000000-0000-0000-0000-000000000006"), Name = "Onboarding 1$", DisplayOrder = 6, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new ProjectStatusMaster { Id = Guid.Parse("A0000000-0000-0000-0000-000000000007"), Name = "Scoped", DisplayOrder = 7, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new ProjectStatusMaster { Id = Guid.Parse("A0000000-0000-0000-0000-000000000008"), Name = "To be Scoped", DisplayOrder = 8, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new ProjectStatusMaster { Id = Guid.Parse("A0000000-0000-0000-0000-000000000009"), Name = "1$ Utilized", DisplayOrder = 9, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );
        }
    }
}
