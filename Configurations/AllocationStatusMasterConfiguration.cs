using HRMS.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRMS.Api.Configurations
{
    public class AllocationStatusMasterConfiguration : IEntityTypeConfiguration<AllocationStatusMaster>
    {
        public void Configure(EntityTypeBuilder<AllocationStatusMaster> builder)
        {
            builder.ToTable("AllocationStatusMasters");
            builder.Property(a => a.Name).HasMaxLength(100).IsRequired();
            builder.Property(a => a.CreatedBy).HasMaxLength(100);
            builder.Property(a => a.ModifiedBy).HasMaxLength(100);
            builder.HasIndex(a => a.Name).IsUnique();
            builder.HasQueryFilter(a => !a.IsDeleted);
        }
    }
}
