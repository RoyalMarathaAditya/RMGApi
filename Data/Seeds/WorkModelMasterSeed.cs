using HRMS.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Data.Seeds
{
    public static class WorkModelMasterSeed
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WorkModelMaster>().HasData(
                new WorkModelMaster { Id = Guid.Parse("20000000-0000-0000-0000-000000000001"), Name = "Hybrid", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new WorkModelMaster { Id = Guid.Parse("20000000-0000-0000-0000-000000000002"), Name = "Remote", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new WorkModelMaster { Id = Guid.Parse("20000000-0000-0000-0000-000000000003"), Name = "WFO", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );
        }
    }
}

