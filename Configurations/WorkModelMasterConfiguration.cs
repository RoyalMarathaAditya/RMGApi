using HRMS.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRMS.Api.Configurations
{
    public class WorkModelMasterConfiguration : IEntityTypeConfiguration<WorkModelMaster>
    {
        public void Configure(EntityTypeBuilder<WorkModelMaster> builder)
        {
            builder.ToTable("WorkModelMasters");
            builder.Property(w => w.Name).HasMaxLength(100).IsRequired();
            builder.Property(w => w.CreatedBy).HasMaxLength(100);
            builder.Property(w => w.ModifiedBy).HasMaxLength(100);
            builder.HasIndex(w => w.Name).IsUnique();
            builder.HasQueryFilter(w => !w.IsDeleted);
        }
    }
}
