using HRMS.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRMS.Api.Configurations
{
    public class ImportAuditConfiguration : IEntityTypeConfiguration<ImportAudit>
    {
        public void Configure(EntityTypeBuilder<ImportAudit> builder)
        {
            builder.ToTable("ImportAudits");
            builder.Property(e => e.FileName).HasMaxLength(255).IsRequired();
            builder.Property(e => e.UploadedBy).HasMaxLength(100);
        }
    }
}
