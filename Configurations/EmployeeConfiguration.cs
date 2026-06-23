using HRMS.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRMS.Api.Configurations
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.ToTable("Employees");

            builder.Property(e => e.EmployeeCode).HasMaxLength(50).IsRequired();
            builder.Property(e => e.FirstName).HasMaxLength(100);
            builder.Property(e => e.LastName).HasMaxLength(100);
            builder.Property(e => e.FullName).HasMaxLength(200).IsRequired();
            builder.Property(e => e.Email).HasMaxLength(150).IsRequired();
            builder.Property(e => e.MobileNumber).HasMaxLength(20);
            builder.Property(e => e.PriorExperience).HasPrecision(8, 2);
            builder.Property(e => e.RelevantExperience).HasPrecision(8, 2);
            builder.Property(e => e.ExperienceYears).HasPrecision(8, 2);
            builder.Property(e => e.RowVersion).IsRowVersion();
            builder.Property(e => e.CreatedBy).HasMaxLength(100);
            builder.Property(e => e.ModifiedBy).HasMaxLength(100);

            builder.HasIndex(e => e.EmployeeCode).IsUnique();
            builder.HasIndex(e => e.Email).IsUnique();
            builder.HasIndex(e => e.PracticeId);
            builder.HasIndex(e => e.StatusId);
            builder.HasIndex(e => e.DesignationId);
            builder.HasIndex(e => e.ClientId);
            builder.HasQueryFilter(e => !e.IsDeleted);

            builder.HasOne(e => e.EmploymentType)
                .WithMany()
                .HasForeignKey(e => e.EmploymentTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Location)
                .WithMany(l => l.Employees)
                .HasForeignKey(e => e.LocationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.WorkModel)
                .WithMany()
                .HasForeignKey(e => e.WorkModelId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Practice)
                .WithMany(p => p.Employees)
                .HasForeignKey(e => e.PracticeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.DepartmentType)
                .WithMany()
                .HasForeignKey(e => e.DepartmentTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.EmployeeStatus)
                .WithMany()
                .HasForeignKey(e => e.StatusId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.ReportingManager)
                .WithMany(e => e.DirectReports)
                .HasForeignKey(e => e.ReportingManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.PracticeHead)
                .WithMany(e => e.PracticeHeadEmployees)
                .HasForeignKey(e => e.PracticeHeadId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Designation)
                .WithMany()
                .HasForeignKey(e => e.DesignationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Client)
                .WithMany()
                .HasForeignKey(e => e.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.OnboardingType)
                .WithMany()
                .HasForeignKey(e => e.OnboardingTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
