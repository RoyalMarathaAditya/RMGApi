using Microsoft.EntityFrameworkCore;
using HRMS.Api.Models;

namespace HRMS.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<ProjectDetails> ProjectDetails { get; set; }
        public DbSet<ProjectDetailsSkill> ProjectDetailsSkills { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            
            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.User)
                .WithMany()
                .HasForeignKey(rt => rt.UserId);



            modelBuilder.Entity<ProjectDetailsSkill>()
                .HasOne(ps => ps.Skill)
                .WithMany(s => s.ProjectSkills)
                .HasForeignKey(ps => ps.SkillId);

            modelBuilder.Entity<ProjectDetailsSkill>()
                .HasOne(ps => ps.ProjectDetails)
                .WithMany(p => p.ProjectSkills)
                .HasForeignKey(ps => ps.ProjectDetailsId);
        }
    }
}
