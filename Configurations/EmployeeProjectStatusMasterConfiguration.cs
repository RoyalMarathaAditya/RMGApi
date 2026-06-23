using HRMS.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRMS.Api.Configurations
{
    public class EmployeeProjectStatusMasterConfiguration : IEntityTypeConfiguration<EmployeeProjectStatusMaster>
    {
        public void Configure(EntityTypeBuilder<EmployeeProjectStatusMaster> builder)
        {
            builder.ToTable("EmployeeProjectStatusMasters");
            builder.Property(e => e.Name).HasMaxLength(100).IsRequired();
            builder.Property(e => e.CreatedBy).HasMaxLength(100);
            builder.Property(e => e.ModifiedBy).HasMaxLength(100);
            builder.HasIndex(e => e.Name).IsUnique();
            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }
}
