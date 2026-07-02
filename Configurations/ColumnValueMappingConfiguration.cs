using HRMS.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRMS.Api.Configurations
{
    public class ColumnValueMappingConfiguration : IEntityTypeConfiguration<ColumnValueMapping>
    {
        public void Configure(EntityTypeBuilder<ColumnValueMapping> builder)
        {
            builder.ToTable("ColumnValueMappings");
            builder.Property(e => e.TargetProperty).HasMaxLength(200).IsRequired();
            builder.Property(e => e.SourceValue).HasMaxLength(200).IsRequired();
            builder.Property(e => e.TargetValue).HasMaxLength(200).IsRequired();
        }
    }
}
