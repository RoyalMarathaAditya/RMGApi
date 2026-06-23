using HRMS.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRMS.Api.Configurations
{
    public class ProjectAllocationConfiguration : IEntityTypeConfiguration<ProjectAllocation>
    {
        public void Configure(EntityTypeBuilder<ProjectAllocation> builder)
        {
            builder.ToTable("ProjectAllocations");

            builder.Property(pa => pa.AllocationPercentage).HasPrecision(5, 2);
            builder.Property(pa => pa.BillablePercentage).HasPrecision(5, 2);
            builder.Property(pa => pa.CreatedBy).HasMaxLength(100);
            builder.Property(pa => pa.ModifiedBy).HasMaxLength(100);
            builder.Property(pa => pa.RowVersion).IsRowVersion();

            builder.HasIndex(pa => pa.EmployeeId);
            builder.HasIndex(pa => pa.ProjectId);
            builder.HasIndex(pa => pa.AllocationStatusId);
            builder.HasIndex(pa => pa.EmployeeProjectStatusId);
            builder.HasIndex(pa => new { pa.EmployeeId, pa.ProjectId, pa.AllocationStatusId });
            builder.HasQueryFilter(pa => !pa.IsDeleted);

            builder.HasOne(pa => pa.Employee)
                .WithMany(e => e.ProjectAllocations)
                .HasForeignKey(pa => pa.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(pa => pa.Project)
                .WithMany(p => p.ProjectAllocations)
                .HasForeignKey(pa => pa.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(pa => pa.EmployeeProjectStatus)
                .WithMany()
                .HasForeignKey(pa => pa.EmployeeProjectStatusId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(pa => pa.AllocationStatus)
                .WithMany()
                .HasForeignKey(pa => pa.AllocationStatusId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
