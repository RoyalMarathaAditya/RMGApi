using HRMS.Api.Models.RMG;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRMS.Api.Configurations
{
    public class ResourceAllocationHistoryConfiguration : IEntityTypeConfiguration<ResourceAllocationHistory>
    {
        public void Configure(EntityTypeBuilder<ResourceAllocationHistory> builder)
        {
            builder.ToTable("ResourceAllocationHistory");
            builder.Property(h => h.ChangeType).HasMaxLength(50).IsRequired();
            builder.Property(h => h.ModifiedBy).HasMaxLength(100).IsRequired();
            builder.Property(h => h.Remarks).HasMaxLength(1000);
            builder.Property(h => h.OldAllocationPercentage).HasPrecision(5, 2);
            builder.Property(h => h.NewAllocationPercentage).HasPrecision(5, 2);
            builder.Property(h => h.OldAllocationStatus).HasMaxLength(50);
            builder.Property(h => h.NewAllocationStatus).HasMaxLength(50);
            builder.HasIndex(h => h.ResourceAllocationId);
            builder.HasIndex(h => h.EmployeeId);
        }
    }
}
