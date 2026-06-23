using HRMS.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRMS.Api.Configurations
{
    public class DesignationMasterConfiguration : IEntityTypeConfiguration<DesignationMaster>
    {
        public void Configure(EntityTypeBuilder<DesignationMaster> builder)
        {
            builder.ToTable("DesignationMasters");
            builder.Property(e => e.Name).HasMaxLength(100).IsRequired();
            builder.Property(e => e.Code).HasMaxLength(20);
            builder.Property(e => e.CreatedBy).HasMaxLength(100);
            builder.Property(e => e.ModifiedBy).HasMaxLength(100);
            builder.HasIndex(e => e.Name).IsUnique();
            builder.HasIndex(e => e.Code).IsUnique().HasFilter("[Code] IS NOT NULL AND [Code] != ''");
            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }
}
