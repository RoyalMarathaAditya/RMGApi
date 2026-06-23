using HRMS.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRMS.Api.Configurations
{
    public class BaseMasterConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseMasterEntity
    {
        private readonly string _tableName;

        public BaseMasterConfiguration(string tableName)
        {
            _tableName = tableName;
        }

        public void Configure(EntityTypeBuilder<T> builder)
        {
            builder.ToTable(_tableName);
            builder.Property(e => e.Name).HasMaxLength(100).IsRequired();
            builder.Property(e => e.CreatedBy).HasMaxLength(100);
            builder.Property(e => e.ModifiedBy).HasMaxLength(100);
            builder.HasIndex(e => e.Name).IsUnique();
            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }
}
