using HRMS.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRMS.Api.Configurations
{
    public class ProbableNextAssignmentMasterConfiguration : IEntityTypeConfiguration<ProbableNextAssignmentMaster>
    {
        public void Configure(EntityTypeBuilder<ProbableNextAssignmentMaster> builder)
        {
            builder.ToTable("ProbableNextAssignmentMasters");
            builder.Property(e => e.Name).HasMaxLength(250).IsRequired();
            builder.Property(e => e.CreatedBy).HasMaxLength(100);
            builder.Property(e => e.ModifiedBy).HasMaxLength(100);
            builder.HasIndex(e => e.Name).IsUnique();
            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }
}
