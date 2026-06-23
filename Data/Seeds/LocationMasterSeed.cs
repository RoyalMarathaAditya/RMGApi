using HRMS.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Data.Seeds
{
    public static class LocationMasterSeed
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Location>().HasData(
                new Location { Id = Guid.Parse("30000000-0000-0000-0000-000000000001"), Name = "Pune", Address = "Pune, India", CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Location { Id = Guid.Parse("30000000-0000-0000-0000-000000000002"), Name = "Hyderabad", Address = "Hyderabad, India", CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Location { Id = Guid.Parse("30000000-0000-0000-0000-000000000003"), Name = "Bhopal", Address = "Bhopal, India", CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Location { Id = Guid.Parse("30000000-0000-0000-0000-000000000004"), Name = "Remote", Address = "Remote", CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );
        }
    }
}

