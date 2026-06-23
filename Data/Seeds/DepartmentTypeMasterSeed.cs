using HRMS.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Data.Seeds
{
    public static class DepartmentTypeMasterSeed
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DepartmentTypeMaster>().HasData(
                new DepartmentTypeMaster { Id = Guid.Parse("B0000000-0000-0000-0000-000000000001"), Name = "Business Unit", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new DepartmentTypeMaster { Id = Guid.Parse("B0000000-0000-0000-0000-000000000002"), Name = "Business Enabler", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );
        }
    }
}

