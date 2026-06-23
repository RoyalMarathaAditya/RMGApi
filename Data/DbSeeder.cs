using HRMS.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Data.Seeders
{
    public static class DbSeeder
    {
        private static readonly Guid StatusActive = Guid.Parse("10000000-0000-0000-0000-000000000001");
        private static readonly Guid FTE = Guid.Parse("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA");
        private static readonly Guid Consultant = Guid.Parse("BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB");
        private static readonly Guid Pune = Guid.Parse("30000000-0000-0000-0000-000000000001");
        private static readonly Guid Hyderabad = Guid.Parse("30000000-0000-0000-0000-000000000002");
        private static readonly Guid Remote = Guid.Parse("30000000-0000-0000-0000-000000000004");
        private static readonly Guid Hybrid = Guid.Parse("20000000-0000-0000-0000-000000000001");
        private static readonly Guid WFO = Guid.Parse("20000000-0000-0000-0000-000000000003");
        private static readonly Guid Microsoft = Guid.Parse("40000000-0000-0000-0000-000000000012");
        private static readonly Guid Data = Guid.Parse("40000000-0000-0000-0000-000000000003");
        private static readonly Guid Java = Guid.Parse("40000000-0000-0000-0000-000000000009");
        private static readonly Guid DigitalAssurance = Guid.Parse("40000000-0000-0000-0000-000000000005");
        private static readonly Guid ManagedServices = Guid.Parse("40000000-0000-0000-0000-000000000010");
        private static readonly Guid BusinessUnit = Guid.Parse("B0000000-0000-0000-0000-000000000001");
        private static readonly Guid BusinessEnabler = Guid.Parse("B0000000-0000-0000-0000-000000000002");
        private static readonly Guid SkillDotNet = Guid.Parse("50000000-0000-0000-0000-000000000001");
        private static readonly Guid SkillAngular = Guid.Parse("50000000-0000-0000-0000-000000000002");
        private static readonly Guid SkillReact = Guid.Parse("50000000-0000-0000-0000-000000000003");
        private static readonly Guid SkillJava = Guid.Parse("50000000-0000-0000-0000-000000000004");
        private static readonly Guid SkillPython = Guid.Parse("50000000-0000-0000-0000-000000000005");
        private static readonly Guid SkillSQL = Guid.Parse("50000000-0000-0000-0000-000000000006");
        private static readonly Guid SkillAzure = Guid.Parse("50000000-0000-0000-0000-000000000007");
        private static readonly Guid SkillAWS = Guid.Parse("50000000-0000-0000-0000-000000000008");
        private static readonly Guid SkillPBI = Guid.Parse("50000000-0000-0000-0000-000000000009");
        private static readonly Guid SkillTesting = Guid.Parse("50000000-0000-0000-0000-000000000010");

        public static async Task SeedAsync(AppDbContext context)
        {
            if (!await context.Practices.AnyAsync(e => e.Name == "AI/ML"))
            {
                context.Practices.Add(new Practice
                {
                    Id = Guid.Parse("40000000-0000-0000-0000-000000000018"),
                    Name = "AI/ML",
                    IsActive = true,
                    CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                });
                context.Practices.Add(new Practice
                {
                    Id = Guid.Parse("40000000-0000-0000-0000-000000000019"),
                    Name = "Cybersecurity",
                    IsActive = true,
                    CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                });
                await context.SaveChangesAsync();
            }

            if (!await context.EmploymentTypeMasters.AnyAsync(e => e.Name == "Contractor"))
            {
                context.EmploymentTypeMasters.Add(new EmploymentTypeMaster
                {
                    Id = Guid.Parse("DDDDDDDD-DDDD-DDDD-DDDD-DDDDDDDDDDDD"),
                    Name = "Contractor",
                    IsActive = true,
                    CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                });
                context.EmploymentTypeMasters.Add(new EmploymentTypeMaster
                {
                    Id = Guid.Parse("EEEEEEEE-EEEE-EEEE-EEEE-EEEEEEEEEEEE"),
                    Name = "Trainee",
                    IsActive = true,
                    CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                });
                await context.SaveChangesAsync();
            }

            if (!await context.Users.AnyAsync())
            {
                context.Users.Add(new User
                {
                    Email = "admin@rmg.com",
                    Name = "Admin User",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    Role = "Admin",
                    IsActive = true
                });
            }

            if (!await context.Employees.AnyAsync())
            {
                var emp1 = new Employee
                {
                    EmployeeCode = "RMG001",
                    FullName = "Aditya Sharma",
                    Email = "aditya.sharma@rmg.com",
                    DOJ = new DateTime(2022, 6, 15),
                    PriorExperience = 5,
                    RelevantExperience = 4,
                    EmploymentTypeId = FTE,
                    LocationId = Pune,
                    WorkModelId = Hybrid,
                    PracticeId = Microsoft,
                    DepartmentTypeId = BusinessUnit,
                    StatusId = StatusActive,
                    DeloitteFitment = true,
                    Engineering = true,
                    MobileNumber = "9876543210",
                    CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                };
                emp1.EmployeeSkills.Add(new EmployeeSkill { SkillId = SkillDotNet });
                emp1.EmployeeSkills.Add(new EmployeeSkill { SkillId = SkillAngular });
                emp1.EmployeeSkills.Add(new EmployeeSkill { SkillId = SkillSQL });
                emp1.EmployeeSkills.Add(new EmployeeSkill { SkillId = SkillAzure });

                var emp2 = new Employee
                {
                    EmployeeCode = "RMG002",
                    FullName = "Priya Patel",
                    Email = "priya.patel@rmg.com",
                    DOJ = new DateTime(2023, 3, 1),
                    PriorExperience = 3,
                    RelevantExperience = 3,
                    EmploymentTypeId = FTE,
                    LocationId = Hyderabad,
                    WorkModelId = WFO,
                    PracticeId = Data,
                    DepartmentTypeId = BusinessUnit,
                    StatusId = StatusActive,
                    DeloitteFitment = true,
                    Engineering = true,
                    MobileNumber = "9876543211",
                    CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                };
                emp2.EmployeeSkills.Add(new EmployeeSkill { SkillId = SkillPython });
                emp2.EmployeeSkills.Add(new EmployeeSkill { SkillId = SkillSQL });
                emp2.EmployeeSkills.Add(new EmployeeSkill { SkillId = SkillPBI });

                var emp3 = new Employee
                {
                    EmployeeCode = "RMG003",
                    FullName = "Rahul Verma",
                    Email = "rahul.verma@rmg.com",
                    DOJ = new DateTime(2024, 1, 10),
                    PriorExperience = 8,
                    RelevantExperience = 6,
                    EmploymentTypeId = FTE,
                    LocationId = Remote,
                    WorkModelId = Hybrid,
                    PracticeId = Java,
                    DepartmentTypeId = BusinessUnit,
                    StatusId = StatusActive,
                    DeloitteFitment = false,
                    Engineering = true,
                    MobileNumber = "9876543212",
                    CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                };
                emp3.EmployeeSkills.Add(new EmployeeSkill { SkillId = SkillJava });
                emp3.EmployeeSkills.Add(new EmployeeSkill { SkillId = SkillReact });
                emp3.EmployeeSkills.Add(new EmployeeSkill { SkillId = SkillAWS });

                var emp4 = new Employee
                {
                    EmployeeCode = "RMG004",
                    FullName = "Sneha Reddy",
                    Email = "sneha.reddy@rmg.com",
                    DOJ = new DateTime(2023, 9, 20),
                    PriorExperience = 2,
                    RelevantExperience = 2,
                    EmploymentTypeId = Consultant,
                    LocationId = Hyderabad,
                    WorkModelId = WFO,
                    PracticeId = DigitalAssurance,
                    DepartmentTypeId = BusinessEnabler,
                    StatusId = StatusActive,
                    DeloitteFitment = true,
                    Engineering = true,
                    MobileNumber = "9876543213",
                    CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                };
                emp4.EmployeeSkills.Add(new EmployeeSkill { SkillId = SkillTesting });
                emp4.EmployeeSkills.Add(new EmployeeSkill { SkillId = SkillPython });

                var emp5 = new Employee
                {
                    EmployeeCode = "RMG005",
                    FullName = "Vikram Singh",
                    Email = "vikram.singh@rmg.com",
                    DOJ = new DateTime(2021, 4, 5),
                    PriorExperience = 10,
                    RelevantExperience = 8,
                    EmploymentTypeId = FTE,
                    LocationId = Pune,
                    WorkModelId = Hybrid,
                    PracticeId = ManagedServices,
                    DepartmentTypeId = BusinessUnit,
                    StatusId = StatusActive,
                    DeloitteFitment = true,
                    Engineering = false,
                    MobileNumber = "9876543214",
                    CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                };
                emp5.EmployeeSkills.Add(new EmployeeSkill { SkillId = SkillDotNet });
                emp5.EmployeeSkills.Add(new EmployeeSkill { SkillId = SkillAzure });
                emp5.EmployeeSkills.Add(new EmployeeSkill { SkillId = SkillSQL });
                emp5.EmployeeSkills.Add(new EmployeeSkill { SkillId = SkillReact });

                context.Employees.AddRange(emp1, emp2, emp3, emp4, emp5);
                await context.SaveChangesAsync();

                // Reload to get auto-generated IDs
                var all = context.Employees.ToList();
                var a1 = all.First(e => e.EmployeeCode == "RMG001");
                var a2 = all.First(e => e.EmployeeCode == "RMG002");
                var a3 = all.First(e => e.EmployeeCode == "RMG003");
                var a4 = all.First(e => e.EmployeeCode == "RMG004");
                var a5 = all.First(e => e.EmployeeCode == "RMG005");

                a1.ReportingManagerId = a5.Id;
                a2.ReportingManagerId = a1.Id;
                a3.ReportingManagerId = a5.Id;
                a4.ReportingManagerId = a1.Id;
                a1.PracticeHeadId = a5.Id;
                a2.PracticeHeadId = a5.Id;
                a3.PracticeHeadId = a5.Id;
                a4.PracticeHeadId = a5.Id;
                await context.SaveChangesAsync();
            }
        }
    }
}
