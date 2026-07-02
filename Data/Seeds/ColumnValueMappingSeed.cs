using HRMS.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Data.Seeds
{
    public static class ColumnValueMappingSeed
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            var now = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            modelBuilder.Entity<ColumnValueMapping>().HasData(
                new ColumnValueMapping { Id = Guid.Parse("B1000000-0000-0000-0000-000000000001"), TargetProperty = "EmployeeType", SourceValue = "Permanent", TargetValue = "FTE", IsActive = true, CreatedOn = now },
                new ColumnValueMapping { Id = Guid.Parse("B1000000-0000-0000-0000-000000000002"), TargetProperty = "EmployeeType", SourceValue = "Contract", TargetValue = "Consultant", IsActive = true, CreatedOn = now },
                new ColumnValueMapping { Id = Guid.Parse("B1000000-0000-0000-0000-000000000003"), TargetProperty = "EmployeeType", SourceValue = "Contractor", TargetValue = "Consultant", IsActive = true, CreatedOn = now },
                new ColumnValueMapping { Id = Guid.Parse("B1000000-0000-0000-0000-000000000004"), TargetProperty = "EmployeeType", SourceValue = "Probation", TargetValue = "FTE", IsActive = true, CreatedOn = now },
                new ColumnValueMapping { Id = Guid.Parse("B1000000-0000-0000-0000-000000000005"), TargetProperty = "ActiveStatus", SourceValue = "Active", TargetValue = "Active", IsActive = true, CreatedOn = now },
                new ColumnValueMapping { Id = Guid.Parse("B1000000-0000-0000-0000-000000000006"), TargetProperty = "ActiveStatus", SourceValue = "Inactive", TargetValue = "Inactive", IsActive = true, CreatedOn = now },
                new ColumnValueMapping { Id = Guid.Parse("B1000000-0000-0000-0000-000000000007"), TargetProperty = "ActiveStatus", SourceValue = "Yes", TargetValue = "Active", IsActive = true, CreatedOn = now },
                new ColumnValueMapping { Id = Guid.Parse("B1000000-0000-0000-0000-000000000008"), TargetProperty = "ActiveStatus", SourceValue = "No", TargetValue = "Inactive", IsActive = true, CreatedOn = now },
                new ColumnValueMapping { Id = Guid.Parse("B1000000-0000-0000-0000-000000000009"), TargetProperty = "ActiveStatus", SourceValue = "Y", TargetValue = "Active", IsActive = true, CreatedOn = now },
                new ColumnValueMapping { Id = Guid.Parse("B1000000-0000-0000-0000-000000000010"), TargetProperty = "ActiveStatus", SourceValue = "N", TargetValue = "Inactive", IsActive = true, CreatedOn = now },
                new ColumnValueMapping { Id = Guid.Parse("B1000000-0000-0000-0000-000000000011"), TargetProperty = "ActiveStatus", SourceValue = "True", TargetValue = "Active", IsActive = true, CreatedOn = now },
                new ColumnValueMapping { Id = Guid.Parse("B1000000-0000-0000-0000-000000000012"), TargetProperty = "ActiveStatus", SourceValue = "False", TargetValue = "Inactive", IsActive = true, CreatedOn = now }
            );
        }
    }
}
