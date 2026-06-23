using HRMS.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRMS.Api.Configurations
{
    public class ClientConfiguration : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {
            builder.ToTable("Clients");

            builder.Property(c => c.Name).HasMaxLength(200).IsRequired();
            builder.Property(c => c.Location).HasMaxLength(200);
            builder.Property(c => c.CreatedBy).HasMaxLength(100);
            builder.Property(c => c.ModifiedBy).HasMaxLength(100);
            builder.Property(c => c.RowVersion).IsRowVersion();

            builder.HasIndex(c => c.Name).IsUnique();
            builder.HasQueryFilter(c => !c.IsDeleted);

            builder.HasOne(c => c.ClientStatus)
                .WithMany()
                .HasForeignKey(c => c.StatusId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
