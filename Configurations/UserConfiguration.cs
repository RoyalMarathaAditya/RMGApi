using HRMS.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRMS.Api.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.Property(u => u.Email).HasMaxLength(100).IsRequired();
            builder.Property(u => u.Name).HasMaxLength(100).IsRequired();
            builder.Property(u => u.PasswordHash).HasMaxLength(255).IsRequired();
            builder.Property(u => u.RoleId).IsRequired();

            builder.HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(u => u.IsFirstLogin).HasDefaultValue(true);
            builder.Property(u => u.IsDefaultPassword).HasDefaultValue(true);
            builder.Property(u => u.PasswordResetRequired).HasDefaultValue(true);

            builder.HasIndex(u => u.RoleId).HasDatabaseName("IX_Users_RoleId");
            builder.HasIndex(u => u.Email).IsUnique().HasDatabaseName("IX_Users_Email");
            builder.HasIndex(u => u.EmployeeId).HasDatabaseName("IX_Users_EmployeeId");
            builder.HasIndex(u => u.IsActive).HasFilter("[IsActive] = 1").HasDatabaseName("IX_Users_IsActive");
            builder.HasIndex(u => u.IsLocked).HasFilter("[IsLocked] = 1").HasDatabaseName("IX_Users_IsLocked");
            builder.HasIndex(u => u.IsDeleted).HasFilter("[IsDeleted] = 0").HasDatabaseName("IX_Users_IsDeleted");
            builder.HasIndex(u => u.CreatedAt).HasDatabaseName("IX_Users_CreatedAt");
            builder.HasIndex(u => u.LastLoginDate).HasDatabaseName("IX_Users_LastLoginDate");
            builder.HasIndex(u => u.PasswordResetRequired).HasFilter("[PasswordResetRequired] = 1").HasDatabaseName("IX_Users_PasswordResetRequired");
        }
    }
}
