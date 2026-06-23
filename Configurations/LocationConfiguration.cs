using HRMS.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRMS.Api.Configurations
{
    public class LocationConfiguration : IEntityTypeConfiguration<Location>
    {
        public void Configure(EntityTypeBuilder<Location> builder)
        {
            builder.ToTable("Locations");
            builder.Property(l => l.Name).HasMaxLength(150).IsRequired();
            builder.Property(l => l.Address).HasMaxLength(500);
            builder.Property(l => l.CreatedBy).HasMaxLength(100);
            builder.Property(l => l.ModifiedBy).HasMaxLength(100);
            builder.HasIndex(l => l.Name).IsUnique();
            builder.HasQueryFilter(l => !l.IsDeleted);
        }
    }
}
