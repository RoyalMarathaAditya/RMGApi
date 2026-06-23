using HRMS.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Data.Seeds
{
    public static class LeaveTypeMasterSeed
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LeaveTypeMaster>().HasData(
                new LeaveTypeMaster { Id = Guid.Parse("60000000-0000-0000-0000-000000000001"), Name = "Maternity", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new LeaveTypeMaster { Id = Guid.Parse("60000000-0000-0000-0000-000000000002"), Name = "Sabbatical", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new LeaveTypeMaster { Id = Guid.Parse("60000000-0000-0000-0000-000000000003"), Name = "Long Leave more than 15 days", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new LeaveTypeMaster { Id = Guid.Parse("60000000-0000-0000-0000-000000000004"), Name = "Earned Leave", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new LeaveTypeMaster { Id = Guid.Parse("60000000-0000-0000-0000-000000000005"), Name = "Sick Leave", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new LeaveTypeMaster { Id = Guid.Parse("60000000-0000-0000-0000-000000000006"), Name = "Optional Holiday", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );
        }
    }
}

