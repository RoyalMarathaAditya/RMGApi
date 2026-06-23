using HRMS.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRMS.Api.Configurations
{
    public class EmployeeLeaveConfiguration : IEntityTypeConfiguration<EmployeeLeave>
    {
        public void Configure(EntityTypeBuilder<EmployeeLeave> builder)
        {
            builder.ToTable("EmployeeLeaves");

            builder.Property(el => el.Remarks).HasMaxLength(500);
            builder.Property(el => el.CreatedBy).HasMaxLength(100);
            builder.Property(el => el.ModifiedBy).HasMaxLength(100);
            builder.Property(el => el.RowVersion).IsRowVersion();
            builder.HasQueryFilter(el => !el.IsDeleted);

            builder.HasOne(el => el.Employee)
                .WithMany(e => e.EmployeeLeaves)
                .HasForeignKey(el => el.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(el => el.LeaveType)
                .WithMany()
                .HasForeignKey(el => el.LeaveTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
