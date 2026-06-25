using HRMS.Api.Models.RMG;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRMS.Api.Configurations
{
    public class ResourceAllocationConfiguration : IEntityTypeConfiguration<ResourceAllocation>
    {
        public void Configure(EntityTypeBuilder<ResourceAllocation> builder)
        {
            builder.ToTable("ResourceAllocations");
            builder.Property(ra => ra.AllocationStatus).HasMaxLength(50).IsRequired();
            builder.Property(ra => ra.AllocationPercentage).HasPrecision(5, 2);
            builder.Property(ra => ra.AllocationType).HasMaxLength(50);
            builder.Property(ra => ra.BillableStatus).HasMaxLength(50);
            builder.Property(ra => ra.Notes).HasMaxLength(1000);
            builder.Property(ra => ra.CreatedBy).HasMaxLength(100);
            builder.Property(ra => ra.ModifiedBy).HasMaxLength(100);
            builder.Property(ra => ra.RowVersion).IsRowVersion();
            builder.HasQueryFilter(ra => !ra.IsDeleted);

            builder.HasOne(ra => ra.Employee)
                .WithMany()
                .HasForeignKey(ra => ra.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ra => ra.Project)
                .WithMany()
                .HasForeignKey(ra => ra.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ra => ra.Client)
                .WithMany()
                .HasForeignKey(ra => ra.ClientId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(ra => new { ra.EmployeeId, ra.ProjectId, ra.AllocationStatus });
        }
    }
}
