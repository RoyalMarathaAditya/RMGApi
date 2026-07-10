using HRMS.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Data.Seeds
{
    public static class RoleMasterSeed
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RoleMaster>().HasData(
                new RoleMaster { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "RMG", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new RoleMaster { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Practice Head", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new RoleMaster { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Name = "Account Manager", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new RoleMaster { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), Name = "CSM", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new RoleMaster { Id = Guid.Parse("55555555-5555-5555-5555-555555555555"), Name = "PMO", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new RoleMaster { Id = Guid.Parse("66666666-6666-6666-6666-666666666666"), Name = "L1 Manager", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new RoleMaster { Id = Guid.Parse("77777777-7777-7777-7777-777777777777"), Name = "Employee", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new RoleMaster { Id = Guid.Parse("88888888-8888-8888-8888-888888888888"), Name = "CxO", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new RoleMaster { Id = Guid.Parse("99999999-9999-9999-9999-999999999999"), Name = "Admin", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new RoleMaster { Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), Name = "Super Admin", IsActive = true, CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );
        }
    }
}

