using HRMS.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Data.Seeds
{
    public static class CSMRevenueTypeMasterSeed
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CSMRevenueType>().HasData(
                new CSMRevenueType { Id = Guid.Parse("E0000000-0000-0000-0000-000000000001"), Name = "Time & Material", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new CSMRevenueType { Id = Guid.Parse("E0000000-0000-0000-0000-000000000002"), Name = "Fixed Price", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new CSMRevenueType { Id = Guid.Parse("E0000000-0000-0000-0000-000000000003"), Name = "Retainer", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new CSMRevenueType { Id = Guid.Parse("E0000000-0000-0000-0000-000000000004"), Name = "License", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new CSMRevenueType { Id = Guid.Parse("E0000000-0000-0000-0000-000000000005"), Name = "Maintenance", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new CSMRevenueType { Id = Guid.Parse("E0000000-0000-0000-0000-000000000006"), Name = "Corporate", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );
        }
    }
}
