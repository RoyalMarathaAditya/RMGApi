using HRMS.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Data.Seeds
{
    public static class DesignationMasterSeed
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DesignationMaster>().HasData(
                new DesignationMaster { Id = Guid.Parse("D0000000-0000-0000-0000-000000000001"), Name = "Software Engineer", Code = "SE", SortOrder = 1, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new DesignationMaster { Id = Guid.Parse("D0000000-0000-0000-0000-000000000002"), Name = "Senior Software Engineer", Code = "SSE", SortOrder = 2, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new DesignationMaster { Id = Guid.Parse("D0000000-0000-0000-0000-000000000003"), Name = "Lead Software Engineer", Code = "LSE", SortOrder = 3, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new DesignationMaster { Id = Guid.Parse("D0000000-0000-0000-0000-000000000004"), Name = "Technical Lead", Code = "TL", SortOrder = 4, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new DesignationMaster { Id = Guid.Parse("D0000000-0000-0000-0000-000000000005"), Name = "Architect", Code = "ARCH", SortOrder = 5, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new DesignationMaster { Id = Guid.Parse("D0000000-0000-0000-0000-000000000006"), Name = "Associate Manager", Code = "ASM", SortOrder = 6, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new DesignationMaster { Id = Guid.Parse("D0000000-0000-0000-0000-000000000007"), Name = "Manager", Code = "MGR", SortOrder = 7, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new DesignationMaster { Id = Guid.Parse("D0000000-0000-0000-0000-000000000008"), Name = "Senior Manager", Code = "SM", SortOrder = 8, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new DesignationMaster { Id = Guid.Parse("D0000000-0000-0000-0000-000000000009"), Name = "Delivery Manager", Code = "DM", SortOrder = 9, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new DesignationMaster { Id = Guid.Parse("D0000000-0000-0000-0000-000000000010"), Name = "Project Manager", Code = "PM", SortOrder = 10, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new DesignationMaster { Id = Guid.Parse("D0000000-0000-0000-0000-000000000011"), Name = "Program Manager", Code = "PGM", SortOrder = 11, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new DesignationMaster { Id = Guid.Parse("D0000000-0000-0000-0000-000000000012"), Name = "Director", Code = "DIR", SortOrder = 12, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new DesignationMaster { Id = Guid.Parse("D0000000-0000-0000-0000-000000000013"), Name = "Vice President", Code = "VP", SortOrder = 13, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new DesignationMaster { Id = Guid.Parse("D0000000-0000-0000-0000-000000000014"), Name = "QA Engineer", Code = "QAE", SortOrder = 14, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new DesignationMaster { Id = Guid.Parse("D0000000-0000-0000-0000-000000000015"), Name = "Senior QA Engineer", Code = "SQAE", SortOrder = 15, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new DesignationMaster { Id = Guid.Parse("D0000000-0000-0000-0000-000000000016"), Name = "QA Lead", Code = "QAL", SortOrder = 16, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new DesignationMaster { Id = Guid.Parse("D0000000-0000-0000-0000-000000000017"), Name = "Business Analyst", Code = "BA", SortOrder = 17, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new DesignationMaster { Id = Guid.Parse("D0000000-0000-0000-0000-000000000018"), Name = "Senior Business Analyst", Code = "SBA", SortOrder = 18, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new DesignationMaster { Id = Guid.Parse("D0000000-0000-0000-0000-000000000019"), Name = "Scrum Master", Code = "SCRUM", SortOrder = 19, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new DesignationMaster { Id = Guid.Parse("D0000000-0000-0000-0000-000000000020"), Name = "Product Owner", Code = "PO", SortOrder = 20, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new DesignationMaster { Id = Guid.Parse("D0000000-0000-0000-0000-000000000021"), Name = "UI/UX Designer", Code = "UIUX", SortOrder = 21, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new DesignationMaster { Id = Guid.Parse("D0000000-0000-0000-0000-000000000022"), Name = "DevOps Engineer", Code = "DEVOPS", SortOrder = 22, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new DesignationMaster { Id = Guid.Parse("D0000000-0000-0000-0000-000000000023"), Name = "Data Engineer", Code = "DE", SortOrder = 23, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new DesignationMaster { Id = Guid.Parse("D0000000-0000-0000-0000-000000000024"), Name = "Data Scientist", Code = "DS", SortOrder = 24, IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );
        }
    }
}