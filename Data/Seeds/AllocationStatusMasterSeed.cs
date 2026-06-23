using HRMS.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Data.Seeds
{
    public static class AllocationStatusMasterSeed
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AllocationStatusMaster>().HasData(
                new AllocationStatusMaster { Id = Guid.Parse("90000000-0000-0000-0000-000000000001"), Name = "Current", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new AllocationStatusMaster { Id = Guid.Parse("90000000-0000-0000-0000-000000000002"), Name = "History", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );
        }
    }
}

