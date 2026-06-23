using HRMS.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Data.Seeds
{
    public static class PricingTypeMasterSeed
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PricingTypeMaster>().HasData(
                new PricingTypeMaster { Id = Guid.Parse("70000000-0000-0000-0000-000000000001"), Name = "FP", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new PricingTypeMaster { Id = Guid.Parse("70000000-0000-0000-0000-000000000002"), Name = "T&M", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new PricingTypeMaster { Id = Guid.Parse("70000000-0000-0000-0000-000000000003"), Name = "Internal", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );
        }
    }
}

