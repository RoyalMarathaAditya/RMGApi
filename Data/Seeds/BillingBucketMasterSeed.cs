using HRMS.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Data.Seeds
{
    public static class BillingBucketMasterSeed
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BillingBucketMaster>().HasData(
                new BillingBucketMaster { Id = Guid.Parse("D0030000-0000-0000-0000-000000000001"), Name = "Billing Team", DisplayOrder = 1, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new BillingBucketMaster { Id = Guid.Parse("D0030000-0000-0000-0000-000000000002"), Name = "Core Team", DisplayOrder = 2, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new BillingBucketMaster { Id = Guid.Parse("D0030000-0000-0000-0000-000000000003"), Name = "Corporate", DisplayOrder = 3, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );
        }
    }
}
