using HRMS.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Data.Seeds
{
    public static class PracticeMasterSeed
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Practice>().HasData(
                new Practice { Id = Guid.Parse("40000000-0000-0000-0000-000000000001"), Name = "Administration", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Practice { Id = Guid.Parse("40000000-0000-0000-0000-000000000002"), Name = "Business Excellence", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Practice { Id = Guid.Parse("40000000-0000-0000-0000-000000000003"), Name = "Data", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Practice { Id = Guid.Parse("40000000-0000-0000-0000-000000000004"), Name = "Delivery Excellence", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Practice { Id = Guid.Parse("40000000-0000-0000-0000-000000000005"), Name = "Digital Assurance", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Practice { Id = Guid.Parse("40000000-0000-0000-0000-000000000006"), Name = "Digital Product Studio", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Practice { Id = Guid.Parse("40000000-0000-0000-0000-000000000007"), Name = "Finance", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Practice { Id = Guid.Parse("40000000-0000-0000-0000-000000000008"), Name = "Human Resources", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Practice { Id = Guid.Parse("40000000-0000-0000-0000-000000000009"), Name = "Java", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Practice { Id = Guid.Parse("40000000-0000-0000-0000-000000000010"), Name = "Managed Services", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Practice { Id = Guid.Parse("40000000-0000-0000-0000-000000000011"), Name = "Management", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Practice { Id = Guid.Parse("40000000-0000-0000-0000-000000000012"), Name = "Microsoft", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Practice { Id = Guid.Parse("40000000-0000-0000-0000-000000000013"), Name = "Mobility", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Practice { Id = Guid.Parse("40000000-0000-0000-0000-000000000014"), Name = "Oracle ERP", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Practice { Id = Guid.Parse("40000000-0000-0000-0000-000000000015"), Name = "RPA", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Practice { Id = Guid.Parse("40000000-0000-0000-0000-000000000016"), Name = "Sales and Marketing", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Practice { Id = Guid.Parse("40000000-0000-0000-0000-000000000017"), Name = "Utilities", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );
        }
    }
}

