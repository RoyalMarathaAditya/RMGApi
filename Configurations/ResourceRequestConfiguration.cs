using HRMS.Api.Models.RMG;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRMS.Api.Configurations
{
    public class ResourceRequestConfiguration : IEntityTypeConfiguration<ResourceRequest>
    {
        public void Configure(EntityTypeBuilder<ResourceRequest> builder)
        {
            builder.ToTable("ResourceRequests");
            builder.Property(r => r.Status).HasMaxLength(50).IsRequired();
            builder.Property(r => r.Priority).HasMaxLength(50);
            builder.Property(r => r.RequiredSkillIds).HasMaxLength(500);
            builder.Property(r => r.Notes).HasMaxLength(1000);
            builder.Property(r => r.CreatedBy).HasMaxLength(100);
            builder.Property(r => r.ModifiedBy).HasMaxLength(100);
            builder.Property(r => r.RowVersion).IsRowVersion();
            builder.HasQueryFilter(r => !r.IsDeleted);

            builder.HasOne(r => r.Project)
                .WithMany()
                .HasForeignKey(r => r.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.RequestedBy)
                .WithMany()
                .HasForeignKey(r => r.RequestedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.Practice)
                .WithMany()
                .HasForeignKey(r => r.PracticeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(r => r.Status);
        }
    }
}
