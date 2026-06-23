using HRMS.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Data.Seeds
{
    public static class SkillMasterSeed
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Skill>().HasData(
                new Skill { Id = Guid.Parse("50000000-0000-0000-0000-000000000001"), Name = ".NET", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Skill { Id = Guid.Parse("50000000-0000-0000-0000-000000000002"), Name = "Angular", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Skill { Id = Guid.Parse("50000000-0000-0000-0000-000000000003"), Name = "React", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Skill { Id = Guid.Parse("50000000-0000-0000-0000-000000000004"), Name = "Java", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Skill { Id = Guid.Parse("50000000-0000-0000-0000-000000000005"), Name = "Python", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Skill { Id = Guid.Parse("50000000-0000-0000-0000-000000000006"), Name = "SQL Server", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Skill { Id = Guid.Parse("50000000-0000-0000-0000-000000000007"), Name = "Azure", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Skill { Id = Guid.Parse("50000000-0000-0000-0000-000000000008"), Name = "AWS", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Skill { Id = Guid.Parse("50000000-0000-0000-0000-000000000009"), Name = "Power BI", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Skill { Id = Guid.Parse("50000000-0000-0000-0000-000000000010"), Name = "Testing", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Skill { Id = Guid.Parse("50000000-0000-0000-0000-000000000011"), Name = "DevOps", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Skill { Id = Guid.Parse("50000000-0000-0000-0000-000000000012"), Name = "Oracle ERP", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Skill { Id = Guid.Parse("50000000-0000-0000-0000-000000000013"), Name = "RPA", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );
        }
    }
}

