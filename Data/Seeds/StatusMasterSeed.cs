using HRMS.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Data.Seeds
{
    public static class StatusMasterSeed
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StatusMaster>().HasData(
                new StatusMaster { Id = Guid.Parse("10000000-0000-0000-0000-000000000001"), Name = "Active", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new StatusMaster { Id = Guid.Parse("10000000-0000-0000-0000-000000000002"), Name = "Inactive", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );
        }
    }
}

