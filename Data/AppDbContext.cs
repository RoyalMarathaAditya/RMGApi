using Microsoft.EntityFrameworkCore;
using HRMS.Api.Models;

namespace HRMS.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        // RMG project entities
        public DbSet<Client> Clients => Set<Client>();
        public DbSet<Location> Locations => Set<Location>();
        public DbSet<ProjectDetails> ProjectDetails => Set<ProjectDetails>();
        public DbSet<Skill> Skills => Set<Skill>();
        public DbSet<ProjectDetailsSkill> ProjectDetailsSkills => Set<ProjectDetailsSkill>();
    }
}
