using HRMS.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRMS.Api.Configurations
{
    public class CSMRevenueTypeConfiguration : IEntityTypeConfiguration<CSMRevenueType>
    {
        public void Configure(EntityTypeBuilder<CSMRevenueType> builder)
        {
            builder.ToTable("CSMRevenueTypes");

            builder.Property(r => r.Name).HasMaxLength(200).IsRequired();
            builder.Property(r => r.CreatedBy).HasMaxLength(100);
            builder.Property(r => r.ModifiedBy).HasMaxLength(100);
            builder.Property(r => r.RowVersion).IsRowVersion();

            builder.HasQueryFilter(r => !r.IsDeleted);
        }
    }
}
