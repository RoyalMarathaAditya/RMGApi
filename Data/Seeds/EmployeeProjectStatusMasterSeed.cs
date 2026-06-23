using HRMS.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Data.Seeds
{
    public static class EmployeeProjectStatusMasterSeed
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EmployeeProjectStatusMaster>().HasData(
                new EmployeeProjectStatusMaster { Id = Guid.Parse("A0000000-0000-0000-0000-000000000001"), Name = "Billable", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new EmployeeProjectStatusMaster { Id = Guid.Parse("A0000000-0000-0000-0000-000000000002"), Name = "Scoped", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new EmployeeProjectStatusMaster { Id = Guid.Parse("A0000000-0000-0000-0000-000000000003"), Name = "To Be Scoped", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new EmployeeProjectStatusMaster { Id = Guid.Parse("A0000000-0000-0000-0000-000000000004"), Name = "Utilized", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new EmployeeProjectStatusMaster { Id = Guid.Parse("A0000000-0000-0000-0000-000000000005"), Name = "Onboarding", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new EmployeeProjectStatusMaster { Id = Guid.Parse("A0000000-0000-0000-0000-000000000006"), Name = "Training", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new EmployeeProjectStatusMaster { Id = Guid.Parse("A0000000-0000-0000-0000-000000000007"), Name = "PIP", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new EmployeeProjectStatusMaster { Id = Guid.Parse("A0000000-0000-0000-0000-000000000008"), Name = "Management", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new EmployeeProjectStatusMaster { Id = Guid.Parse("A0000000-0000-0000-0000-000000000009"), Name = "Long Leave", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new EmployeeProjectStatusMaster { Id = Guid.Parse("A0000000-0000-0000-0000-000000000010"), Name = "1$ Utilized", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new EmployeeProjectStatusMaster { Id = Guid.Parse("A0000000-0000-0000-0000-000000000011"), Name = "Shadowing", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );
        }
    }
}

