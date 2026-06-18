using HRMS.Api.Models;

namespace HRMS.Api.Data.Seeders
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            // 🔥 1. DESIGNATIONS
            if (!context.Designations.Any())
            {
                context.Designations.AddRange(
                    new Designation { Name = "CEO" },
                    new Designation { Name = "CTO" },
                    new Designation { Name = "Tech Lead" },
                    new Designation { Name = "Senior Developer" },
                    new Designation { Name = "Junior Developer" }
                );
            }

            // 🔥 2. PRACTICES
            if (!context.Practices.Any())
            {
                context.Practices.AddRange(
                    new Practice { Name = "Digital Engineering", Description = "Software Delivery", IsActive = true },
                    new Practice { Name = "Cloud", Description = "Cloud Services", IsActive = true }
                );
            }

            // 🔥 3. LOCATIONS
            if (!context.Locations.Any())
            {
                context.Locations.AddRange(
                    new Location { Name = "Pune", Address = "India" },
                    new Location { Name = "Bangalore", Address = "India" }
                );
            }

            await context.SaveChangesAsync();

            // 🔥 4. MANAGEMENT LEADERS
            if (!context.ManagementLeaders.Any())
            {
                context.ManagementLeaders.AddRange(
                    new ManagementLeader { LeaderCode = "ML001", FullName = "John Smith", Email = "john@hrms.com", Role = "L1" },
                    new ManagementLeader { LeaderCode = "ML002", FullName = "Alice Brown", Email = "alice@hrms.com", Role = "L2" },
                    new ManagementLeader { LeaderCode = "ML003", FullName = "Mike Ross", Email = "mike@hrms.com", Role = "PM" }
                );
            }

            await context.SaveChangesAsync();

            // 🔥 5. EMPLOYEES
            if (!context.Employees.Any())
            {
                context.Employees.AddRange(
                    new Employee
                    {
                        EmployeeCode = "E001",
                        FullName = "Rahul Sharma",
                        Email = "rahul@hrms.com",
                        DateOfJoining = DateTime.UtcNow,
                        PriorExperience = 2,
                        CompanyExperience = 1,
                        TotalExperience = 3,
                        Status = "Active",
                        DesignationId = 1,
                        PracticeId = 1,
                        LocationId = 1
                    },
                    new Employee
                    {
                        EmployeeCode = "E002",
                        FullName = "Priya Verma",
                        Email = "priya@hrms.com",
                        DateOfJoining = DateTime.UtcNow,
                        PriorExperience = 3,
                        CompanyExperience = 2,
                        TotalExperience = 5,
                        Status = "Active",
                        DesignationId = 2,
                        PracticeId = 1,
                        LocationId = 2
                    }
                );
            }

            await context.SaveChangesAsync();

            // 🔥 6. EMPLOYEE HIERARCHY
            if (!context.EmployeeHierarchies.Any())
            {
                context.EmployeeHierarchies.AddRange(
                    new EmployeeHierarchy
                    {
                        EmployeeId = 1,
                        ManagementLeaderId = 1,
                        ManagerType = ManagerType.L1Manager
                    },
                    new EmployeeHierarchy
                    {
                        EmployeeId = 1,
                        ManagementLeaderId = 2,
                        ManagerType = ManagerType.L2Manager
                    },
                    new EmployeeHierarchy
                    {
                        EmployeeId = 2,
                        ManagementLeaderId = 3,
                        ManagerType = ManagerType.ProjectManager
                    }
                );
            }

            await context.SaveChangesAsync();
        }
    }
}