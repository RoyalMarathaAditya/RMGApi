using HRMS.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRMS.Api.Configurations
{
    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.ToTable("Projects");

            builder.Property(p => p.ProjectName).HasMaxLength(200).IsRequired();
            builder.Property(p => p.ProjectCode).HasMaxLength(50);
            builder.Property(p => p.ProjectManager).HasMaxLength(200);
            builder.Property(p => p.DeliveryHead).HasMaxLength(200);
            builder.Property(p => p.CSM).HasMaxLength(200);
            builder.Property(p => p.Description).HasMaxLength(1000);
            builder.Property(p => p.ProjectStartDate).IsRequired();
            builder.Property(p => p.ProjectEndDate).IsRequired();
            builder.Property(p => p.CreatedBy).HasMaxLength(100);
            builder.Property(p => p.ModifiedBy).HasMaxLength(100);
            builder.Property(p => p.RowVersion).IsRowVersion();

            builder.HasIndex(p => p.ClientId);
            builder.HasQueryFilter(p => !p.IsDeleted);

            builder.HasOne(p => p.Client)
                .WithMany(c => c.Projects)
                .HasForeignKey(p => p.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.CSMRevenueType)
                .WithMany(r => r.Projects)
                .HasForeignKey(p => p.CSMRevenueTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
