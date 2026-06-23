using HRMS.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Data.Seeds
{
    public static class ProjectTypeMasterSeed
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProjectTypeMaster>().HasData(
                new ProjectTypeMaster { Id = Guid.Parse("80000000-0000-0000-0000-000000000001"), Name = "FP", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new ProjectTypeMaster { Id = Guid.Parse("80000000-0000-0000-0000-000000000002"), Name = "T&M", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new ProjectTypeMaster { Id = Guid.Parse("80000000-0000-0000-0000-000000000003"), Name = "Internal", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );
        }
    }
}

