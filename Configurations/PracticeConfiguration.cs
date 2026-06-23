using HRMS.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRMS.Api.Configurations
{
    public class PracticeConfiguration : IEntityTypeConfiguration<Practice>
    {
        public void Configure(EntityTypeBuilder<Practice> builder)
        {
            builder.ToTable("Practices");
            builder.Property(p => p.Name).HasMaxLength(100).IsRequired();
            builder.Property(p => p.Description).HasMaxLength(500);
            builder.Property(p => p.CreatedBy).HasMaxLength(100);
            builder.Property(p => p.ModifiedBy).HasMaxLength(100);
            builder.HasIndex(p => p.Name).IsUnique();
            builder.HasQueryFilter(p => !p.IsDeleted);

            builder.HasOne(p => p.PracticeHead)
                .WithMany()
                .HasForeignKey(p => p.PracticeHeadId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
