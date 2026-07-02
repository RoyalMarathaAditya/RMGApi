using HRMS.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Data.Seeds
{
    public static class AgeingBucketMasterSeed
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AgeingBucketMaster>().HasData(
                new AgeingBucketMaster { Id = Guid.Parse("D0040001-0000-0000-0000-000000000001"), Name = "< 1 month", DisplayOrder = 1, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new AgeingBucketMaster { Id = Guid.Parse("D0040001-0000-0000-0000-000000000002"), Name = "1-3 months", DisplayOrder = 2, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new AgeingBucketMaster { Id = Guid.Parse("D0040001-0000-0000-0000-000000000003"), Name = "3 to 6 months", DisplayOrder = 3, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new AgeingBucketMaster { Id = Guid.Parse("D0040001-0000-0000-0000-000000000004"), Name = "> 6 months", DisplayOrder = 4, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );
        }
    }
}
