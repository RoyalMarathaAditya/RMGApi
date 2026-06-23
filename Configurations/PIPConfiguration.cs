using HRMS.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRMS.Api.Configurations
{
    public class PIPConfiguration : IEntityTypeConfiguration<PIP>
    {
        public void Configure(EntityTypeBuilder<PIP> builder)
        {
            builder.ToTable("PIPs");

            builder.Property(p => p.Reason).HasMaxLength(1000);
            builder.Property(p => p.Status).HasMaxLength(50);
            builder.Property(p => p.Remarks).HasMaxLength(1000);
            builder.Property(p => p.CreatedBy).HasMaxLength(100);
            builder.Property(p => p.ModifiedBy).HasMaxLength(100);
            builder.Property(p => p.RowVersion).IsRowVersion();
            builder.HasQueryFilter(p => !p.IsDeleted);

            builder.HasOne(p => p.Employee)
                .WithMany(e => e.PIPs)
                .HasForeignKey(p => p.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
