using HRMS.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Data.Seeds
{
    public static class ClientMasterSeed
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>().HasData(
                new Client { Id = 1, Name = "Aivontis", StatusId = Guid.Parse("10000000-0000-0000-0000-000000000001"), Location = null, CreatedOn = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc), CreatedBy = "System", IsDeleted = false },
                new Client { Id = 2, Name = "Austills", StatusId = Guid.Parse("10000000-0000-0000-0000-000000000001"), Location = null, CreatedOn = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc), CreatedBy = "System", IsDeleted = false },
                new Client { Id = 3, Name = "Axiad", StatusId = Guid.Parse("10000000-0000-0000-0000-000000000001"), Location = null, CreatedOn = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc), CreatedBy = "System", IsDeleted = false },
                new Client { Id = 4, Name = "BCT - Nama", StatusId = Guid.Parse("10000000-0000-0000-0000-000000000001"), Location = null, CreatedOn = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc), CreatedBy = "System", IsDeleted = false },
                new Client { Id = 5, Name = "Beyondsoft", StatusId = Guid.Parse("10000000-0000-0000-0000-000000000001"), Location = null, CreatedOn = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc), CreatedBy = "System", IsDeleted = false },
                new Client { Id = 6, Name = "Biolabs", StatusId = Guid.Parse("10000000-0000-0000-0000-000000000001"), Location = null, CreatedOn = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc), CreatedBy = "System", IsDeleted = false },
                new Client { Id = 7, Name = "BIS", StatusId = Guid.Parse("10000000-0000-0000-0000-000000000001"), Location = null, CreatedOn = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc), CreatedBy = "System", IsDeleted = false },
                new Client { Id = 8, Name = "Deloitte", StatusId = Guid.Parse("10000000-0000-0000-0000-000000000001"), Location = null, CreatedOn = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc), CreatedBy = "System", IsDeleted = false },
                new Client { Id = 9, Name = "Doxa", StatusId = Guid.Parse("10000000-0000-0000-0000-000000000001"), Location = null, CreatedOn = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc), CreatedBy = "System", IsDeleted = false },
                new Client { Id = 10, Name = "Experis", StatusId = Guid.Parse("10000000-0000-0000-0000-000000000001"), Location = null, CreatedOn = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc), CreatedBy = "System", IsDeleted = false },
                new Client { Id = 11, Name = "Globe Tele Services", StatusId = Guid.Parse("10000000-0000-0000-0000-000000000001"), Location = null, CreatedOn = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc), CreatedBy = "System", IsDeleted = false },
                new Client { Id = 12, Name = "Growth IT", StatusId = Guid.Parse("10000000-0000-0000-0000-000000000001"), Location = null, CreatedOn = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc), CreatedBy = "System", IsDeleted = false },
                new Client { Id = 13, Name = "ING Financial Services LLC", StatusId = Guid.Parse("10000000-0000-0000-0000-000000000001"), Location = null, CreatedOn = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc), CreatedBy = "System", IsDeleted = false },
                new Client { Id = 14, Name = "Miyahuna", StatusId = Guid.Parse("10000000-0000-0000-0000-000000000001"), Location = null, CreatedOn = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc), CreatedBy = "System", IsDeleted = false },
                new Client { Id = 15, Name = "NewVision", StatusId = Guid.Parse("10000000-0000-0000-0000-000000000001"), Location = null, CreatedOn = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc), CreatedBy = "System", IsDeleted = false },
                new Client { Id = 16, Name = "Open Road App LLC", StatusId = Guid.Parse("10000000-0000-0000-0000-000000000001"), Location = null, CreatedOn = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc), CreatedBy = "System", IsDeleted = false },
                new Client { Id = 17, Name = "Oracle", StatusId = Guid.Parse("10000000-0000-0000-0000-000000000001"), Location = null, CreatedOn = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc), CreatedBy = "System", IsDeleted = false },
                new Client { Id = 18, Name = "RKON", StatusId = Guid.Parse("10000000-0000-0000-0000-000000000001"), Location = null, CreatedOn = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc), CreatedBy = "System", IsDeleted = false },
                new Client { Id = 19, Name = "RSM", StatusId = Guid.Parse("10000000-0000-0000-0000-000000000001"), Location = null, CreatedOn = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc), CreatedBy = "System", IsDeleted = false },
                new Client { Id = 20, Name = "Seasalt", StatusId = Guid.Parse("10000000-0000-0000-0000-000000000001"), Location = null, CreatedOn = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc), CreatedBy = "System", IsDeleted = false },
                new Client { Id = 21, Name = "SoftServe", StatusId = Guid.Parse("10000000-0000-0000-0000-000000000001"), Location = null, CreatedOn = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc), CreatedBy = "System", IsDeleted = false },
                new Client { Id = 22, Name = "Sports Media Agency", StatusId = Guid.Parse("10000000-0000-0000-0000-000000000001"), Location = null, CreatedOn = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc), CreatedBy = "System", IsDeleted = false },
                new Client { Id = 23, Name = "USP", StatusId = Guid.Parse("10000000-0000-0000-0000-000000000001"), Location = null, CreatedOn = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc), CreatedBy = "System", IsDeleted = false },
                new Client { Id = 24, Name = "Votal", StatusId = Guid.Parse("10000000-0000-0000-0000-000000000001"), Location = null, CreatedOn = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc), CreatedBy = "System", IsDeleted = false }
            );
        }
    }
}
