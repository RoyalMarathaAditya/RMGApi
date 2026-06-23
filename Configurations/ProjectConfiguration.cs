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
            builder.Property(p => p.Description).HasMaxLength(1000);
            builder.Property(p => p.CreatedBy).HasMaxLength(100);
            builder.Property(p => p.ModifiedBy).HasMaxLength(100);
            builder.Property(p => p.RowVersion).IsRowVersion();

            builder.HasIndex(p => p.ClientId);
            builder.HasQueryFilter(p => !p.IsDeleted);

            builder.HasOne(p => p.Client)
                .WithMany(c => c.Projects)
                .HasForeignKey(p => p.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.ProjectType)
                .WithMany()
                .HasForeignKey(p => p.ProjectTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.PricingType)
                .WithMany()
                .HasForeignKey(p => p.PricingTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Practice)
                .WithMany()
                .HasForeignKey(p => p.PracticeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.ProjectManager)
                .WithMany(e => e.ManagedProjects)
                .HasForeignKey(p => p.ProjectManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.CSM)
                .WithMany(e => e.CSMProjects)
                .HasForeignKey(p => p.CSMId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
