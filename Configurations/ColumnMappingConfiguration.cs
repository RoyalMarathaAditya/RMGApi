using HRMS.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRMS.Api.Configurations
{
    public class ColumnMappingConfiguration : IEntityTypeConfiguration<ColumnMapping>
    {
        public void Configure(EntityTypeBuilder<ColumnMapping> builder)
        {
            builder.ToTable("ColumnMappings");
            builder.Property(e => e.SourceColumn).HasMaxLength(200).IsRequired();
            builder.Property(e => e.TargetProperty).HasMaxLength(200).IsRequired();
            builder.Property(e => e.TargetDisplayName).HasMaxLength(200);
            builder.Property(e => e.DataType).HasMaxLength(50).IsRequired();
            builder.HasIndex(e => e.SourceColumn).IsUnique().HasFilter("[IsActive] = 1");
        }
    }
}
