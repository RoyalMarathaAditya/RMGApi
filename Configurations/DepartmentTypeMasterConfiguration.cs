using HRMS.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRMS.Api.Configurations
{
    public class DepartmentTypeMasterConfiguration : IEntityTypeConfiguration<DepartmentTypeMaster>
    {
        public void Configure(EntityTypeBuilder<DepartmentTypeMaster> builder)
        {
            builder.ToTable("DepartmentTypeMasters");
            builder.Property(d => d.Name).HasMaxLength(100).IsRequired();
            builder.Property(d => d.CreatedBy).HasMaxLength(100);
            builder.Property(d => d.ModifiedBy).HasMaxLength(100);
            builder.HasIndex(d => d.Name).IsUnique();
            builder.HasQueryFilter(d => !d.IsDeleted);
        }
    }
}
