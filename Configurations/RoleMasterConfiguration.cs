using HRMS.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRMS.Api.Configurations
{
    public class RoleMasterConfiguration : IEntityTypeConfiguration<RoleMaster>
    {
        public void Configure(EntityTypeBuilder<RoleMaster> builder)
        {
            builder.ToTable("RoleMasters");
            builder.Property(r => r.Name).HasMaxLength(100).IsRequired();
            builder.Property(r => r.CreatedBy).HasMaxLength(100);
            builder.Property(r => r.ModifiedBy).HasMaxLength(100);
            builder.HasIndex(r => r.Name).IsUnique();
            builder.HasQueryFilter(r => !r.IsDeleted);
        }
    }
}
