using HRMS.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Data.Seeds
{
    public static class BillableDateProbabilityMasterSeed
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BillableDateProbabilityMaster>().HasData(
                new BillableDateProbabilityMaster { Id = Guid.Parse("D0010000-0000-0000-0000-000000000001"), Name = "Billable", DisplayOrder = 1, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new BillableDateProbabilityMaster { Id = Guid.Parse("D0010000-0000-0000-0000-000000000002"), Name = "Billable - Cost covered", DisplayOrder = 2, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new BillableDateProbabilityMaster { Id = Guid.Parse("D0010000-0000-0000-0000-000000000003"), Name = "Confirmed", DisplayOrder = 3, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new BillableDateProbabilityMaster { Id = Guid.Parse("D0010000-0000-0000-0000-000000000004"), Name = "Core Team", DisplayOrder = 4, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new BillableDateProbabilityMaster { Id = Guid.Parse("D0010000-0000-0000-0000-000000000005"), Name = "Corporate", DisplayOrder = 5, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new BillableDateProbabilityMaster { Id = Guid.Parse("D0010000-0000-0000-0000-000000000006"), Name = "Long Leave", DisplayOrder = 6, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new BillableDateProbabilityMaster { Id = Guid.Parse("D0010000-0000-0000-0000-000000000007"), Name = "Most Likely Billable", DisplayOrder = 7, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new BillableDateProbabilityMaster { Id = Guid.Parse("D0010000-0000-0000-0000-000000000008"), Name = "Non-Billable", DisplayOrder = 8, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );
        }
    }
}
