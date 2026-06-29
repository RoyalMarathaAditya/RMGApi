using HRMS.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Data.Seeds
{
    public static class AgeingBucketMasterSeed
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AgeingBucketMaster>().HasData(
                new AgeingBucketMaster { Id = Guid.Parse("D0040001-0000-0000-0000-000000000001"), Name = "0-30 Days", DisplayOrder = 1, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new AgeingBucketMaster { Id = Guid.Parse("D0040001-0000-0000-0000-000000000002"), Name = "31-60 Days", DisplayOrder = 2, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new AgeingBucketMaster { Id = Guid.Parse("D0040001-0000-0000-0000-000000000003"), Name = "61-90 Days", DisplayOrder = 3, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new AgeingBucketMaster { Id = Guid.Parse("D0040001-0000-0000-0000-000000000004"), Name = "91-120 Days", DisplayOrder = 4, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new AgeingBucketMaster { Id = Guid.Parse("D0040001-0000-0000-0000-000000000005"), Name = "120+ Days", DisplayOrder = 5, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );
        }
    }
}
