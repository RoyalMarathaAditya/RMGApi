using HRMS.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRMS.Api.Configurations
{
    public class StatusMasterConfiguration : IEntityTypeConfiguration<StatusMaster>
    {
        public void Configure(EntityTypeBuilder<StatusMaster> builder)
        {
            builder.ToTable("StatusMasters");
            builder.Property(s => s.Name).HasMaxLength(100).IsRequired();
            builder.Property(s => s.CreatedBy).HasMaxLength(100);
            builder.Property(s => s.ModifiedBy).HasMaxLength(100);
            builder.HasIndex(s => s.Name).IsUnique();
            builder.HasQueryFilter(s => !s.IsDeleted);
        }
    }
}
