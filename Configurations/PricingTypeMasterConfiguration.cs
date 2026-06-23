using HRMS.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRMS.Api.Configurations
{
    public class PricingTypeMasterConfiguration : IEntityTypeConfiguration<PricingTypeMaster>
    {
        public void Configure(EntityTypeBuilder<PricingTypeMaster> builder)
        {
            builder.ToTable("PricingTypeMasters");
            builder.Property(p => p.Name).HasMaxLength(100).IsRequired();
            builder.Property(p => p.CreatedBy).HasMaxLength(100);
            builder.Property(p => p.ModifiedBy).HasMaxLength(100);
            builder.HasIndex(p => p.Name).IsUnique();
            builder.HasQueryFilter(p => !p.IsDeleted);
        }
    }
}
