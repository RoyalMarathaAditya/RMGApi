using HRMS.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRMS.Api.Configurations
{
    public class SubPracticeMasterConfiguration : IEntityTypeConfiguration<SubPracticeMaster>
    {
        public void Configure(EntityTypeBuilder<SubPracticeMaster> builder)
        {
            builder.ToTable("SubPracticeMasters");
            builder.Property(p => p.Name).HasMaxLength(100).IsRequired();
            builder.Property(p => p.CreatedBy).HasMaxLength(100);
            builder.Property(p => p.ModifiedBy).HasMaxLength(100);
            builder.HasIndex(p => new { p.Name, p.PracticeId }).IsUnique();
            builder.HasQueryFilter(p => !p.IsDeleted);

            builder.HasOne(p => p.Practice)
                .WithMany()
                .HasForeignKey(p => p.PracticeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
