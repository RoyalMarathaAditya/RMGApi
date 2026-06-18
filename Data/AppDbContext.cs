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

        public DbSet<Role> Roles { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }

        // RMG Masters
        public DbSet<Employee> Employees { get; set; }

        public DbSet<Designation> Designations { get; set; }

        public DbSet<Practice> Practices { get; set; }

        public DbSet<SubPractice> SubPractices { get; set; }

        public DbSet<Skill> Skills { get; set; }

        public DbSet<EmployeeSkill> EmployeeSkills { get; set; }

        public DbSet<Client> Clients { get; set; }

        public DbSet<Location> Locations { get; set; }

        public DbSet<Project> Projects { get; set; }

        public DbSet<ProjectSkill> ProjectSkills { get; set; }

        public DbSet<ResourceAllocation> ResourceAllocations { get; set; }

        public DbSet<FutureAssignment> FutureAssignments { get; set; }

        public DbSet<EmployeeHierarchy> EmployeeHierarchies { get; set; }

        public DbSet<ManagementLeader> ManagementLeaders { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Refresh Token
            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.User)
                .WithMany()
                .HasForeignKey(rt => rt.UserId);

            // Employee Manager Hierarchy
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Manager)
                .WithMany(e => e.DirectReports)
                .HasForeignKey(e => e.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Project Manager
            modelBuilder.Entity<Project>()
                .HasOne(p => p.ProjectManager)
                .WithMany(e => e.ManagedProjects)
                .HasForeignKey(p => p.ProjectManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Delivery Head
            modelBuilder.Entity<Project>()
                .HasOne(p => p.DeliveryHead)
                .WithMany(e => e.DeliveryProjects)
                .HasForeignKey(p => p.DeliveryHeadId)
                .OnDelete(DeleteBehavior.Restrict);

            // EmployeeSkill
            modelBuilder.Entity<EmployeeSkill>()
                .HasIndex(x => new { x.EmployeeId, x.SkillId })
                .IsUnique();

            // ProjectSkill
            modelBuilder.Entity<ProjectSkill>()
                .HasIndex(x => new { x.ProjectId, x.SkillId })
                .IsUnique();

            // Unique Email
            modelBuilder.Entity<Employee>()
                .HasIndex(x => x.Email)
                .IsUnique();

            // Masters Unique Names
            modelBuilder.Entity<Designation>()
                .HasIndex(x => x.Name)
                .IsUnique();

            modelBuilder.Entity<Practice>()
                .HasIndex(x => x.Name)
                .IsUnique();

            modelBuilder.Entity<Skill>()
                .HasIndex(x => x.Name)
                .IsUnique();

            modelBuilder.Entity<Client>()
                .HasIndex(x => x.Name)
                .IsUnique();

            // Employee
            modelBuilder.Entity<Employee>()
                .Property(x => x.PriorExperience)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Employee>()
                .Property(x => x.CompanyExperience)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Employee>()
                .Property(x => x.TotalExperience)
                .HasPrecision(5, 2);

            // EmployeeSkill
            modelBuilder.Entity<EmployeeSkill>()
                .Property(x => x.ExperienceInYears)
                .HasPrecision(5, 2);

            // ResourceAllocation
            modelBuilder.Entity<ResourceAllocation>()
                .Property(x => x.AllocationPercentage)
                .HasPrecision(5, 2);

            modelBuilder.Entity<ResourceAllocation>()
                .Property(x => x.BillablePercentage)
                .HasPrecision(5, 2);

            modelBuilder.Entity<ResourceAllocation>()
                .HasOne(ra => ra.Employee)
                .WithMany(e => e.ResourceAllocations)
                .HasForeignKey(ra => ra.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Resource Allocation -> Project
            modelBuilder.Entity<ResourceAllocation>()
                .HasOne(ra => ra.Project)
                .WithMany(p => p.ResourceAllocations)
                .HasForeignKey(ra => ra.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FutureAssignment>()
                .HasOne(fa => fa.Employee)
                .WithMany(e => e.FutureAssignments)
                .HasForeignKey(fa => fa.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FutureAssignment>()
                .HasOne(fa => fa.Project)
                .WithMany(p => p.FutureAssignments)
                .HasForeignKey(fa => fa.ProjectId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<EmployeeHierarchy>()
                .HasOne(x => x.Employee)
                .WithMany()
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EmployeeHierarchy>()
                .HasOne(x => x.Employee)
                .WithMany()
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EmployeeHierarchy>()
                .HasOne(x => x.ManagementLeader)
                .WithMany()
                .HasForeignKey(x => x.ManagementLeaderId)
                .OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<EmployeeHierarchy>()
            //    .HasOne(eh => eh.Employee)
            //    .WithMany(e => e.EmployeeHierarchies)
            //    .HasForeignKey(eh => eh.EmployeeId)
            //    .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EmployeeHierarchy>()
                 .HasOne(eh => eh.ManagementLeader)
                 .WithMany()
                 .HasForeignKey(eh => eh.ManagementLeaderId)
                 .OnDelete(DeleteBehavior.Restrict);
                     }
    }
}
