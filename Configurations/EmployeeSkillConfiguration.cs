using HRMS.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRMS.Api.Configurations
{
    public class EmployeeSkillConfiguration : IEntityTypeConfiguration<EmployeeSkill>
    {
        public void Configure(EntityTypeBuilder<EmployeeSkill> builder)
        {
            builder.ToTable("EmployeeSkills");

            builder.Property(e => e.RelevantExperience).HasPrecision(8, 2);

            builder.HasIndex(e => new { e.EmployeeId, e.SkillId }).IsUnique();

            builder.HasOne(e => e.Employee)
                .WithMany(e => e.EmployeeSkills)
                .HasForeignKey(e => e.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Skill)
                .WithMany(s => s.EmployeeSkills)
                .HasForeignKey(e => e.SkillId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
