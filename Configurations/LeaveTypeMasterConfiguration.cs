using HRMS.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRMS.Api.Configurations
{
    public class LeaveTypeMasterConfiguration : IEntityTypeConfiguration<LeaveTypeMaster>
    {
        public void Configure(EntityTypeBuilder<LeaveTypeMaster> builder)
        {
            builder.ToTable("LeaveTypeMasters");
            builder.Property(l => l.Name).HasMaxLength(100).IsRequired();
            builder.Property(l => l.CreatedBy).HasMaxLength(100);
            builder.Property(l => l.ModifiedBy).HasMaxLength(100);
            builder.HasIndex(l => l.Name).IsUnique();
            builder.HasQueryFilter(l => !l.IsDeleted);
        }
    }
}
