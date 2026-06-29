using HRMS.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Data.Seeds
{
    public static class CurrentBillingStatusMasterSeed
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CurrentBillingStatusMaster>().HasData(
                new CurrentBillingStatusMaster { Id = Guid.Parse("D0020000-0000-0000-0000-000000000001"), Name = "Available Pool", DisplayOrder = 1, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new CurrentBillingStatusMaster { Id = Guid.Parse("D0020000-0000-0000-0000-000000000002"), Name = "Billable", DisplayOrder = 2, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new CurrentBillingStatusMaster { Id = Guid.Parse("D0020000-0000-0000-0000-000000000003"), Name = "Billable - Cost Covered", DisplayOrder = 3, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new CurrentBillingStatusMaster { Id = Guid.Parse("D0020000-0000-0000-0000-000000000004"), Name = "Buffer", DisplayOrder = 4, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new CurrentBillingStatusMaster { Id = Guid.Parse("D0020000-0000-0000-0000-000000000005"), Name = "Confirmed", DisplayOrder = 5, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new CurrentBillingStatusMaster { Id = Guid.Parse("D0020000-0000-0000-0000-000000000006"), Name = "Core Team", DisplayOrder = 6, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new CurrentBillingStatusMaster { Id = Guid.Parse("D0020000-0000-0000-0000-000000000007"), Name = "Corporate", DisplayOrder = 7, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new CurrentBillingStatusMaster { Id = Guid.Parse("D0020000-0000-0000-0000-000000000008"), Name = "Long Leave", DisplayOrder = 8, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new CurrentBillingStatusMaster { Id = Guid.Parse("D0020000-0000-0000-0000-000000000009"), Name = "Most Likely Billable", DisplayOrder = 9, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new CurrentBillingStatusMaster { Id = Guid.Parse("D0020000-0000-0000-0000-00000000000A"), Name = "To Be Optimised / Exit / PIP", DisplayOrder = 10, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );
        }
    }
}
