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

        private static readonly Guid PracDataAI = Guid.Parse("50000000-0000-0000-0000-000000000001");
        private static readonly Guid PracHR = Guid.Parse("50000000-0000-0000-0000-000000000002");
        private static readonly Guid PracInternalIT = Guid.Parse("50000000-0000-0000-0000-000000000003");
        private static readonly Guid PracLAMP = Guid.Parse("50000000-0000-0000-0000-000000000004");

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

            var seedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var newPractices = new (Guid Id, string Name)[]
            {
                (PracDataAI, "Data and AI"),
                (PracHR, "HR"),
                (PracInternalIT, "Internal IT"),
                (PracLAMP, "LAMP"),
            };
            foreach (var (id, name) in newPractices)
            {
                if (!await context.Practices.AnyAsync(e => e.Name == name))
                {
                    context.Practices.Add(new Practice { Id = id, Name = name, IsActive = true, CreatedOn = seedDate });
                }
            }
            await context.SaveChangesAsync();

            if (!await context.Set<SubPracticeMaster>().AnyAsync())
            {
                var now = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                var pracMap = await context.Practices.Where(p => !p.IsDeleted).ToDictionaryAsync(p => p.Name, p => p.Id);

                var subPracticeDefs = new (Guid Id, string Name, string PracticeName)[]
                {
                    (Guid.Parse("337ae42c-b8ed-4850-9821-912807115dc3"), "Administration", "Administration"),
                    (Guid.Parse("bb061004-e7e3-4144-84f8-42d4acce7bca"), "Android", "Mobility"),
                    (Guid.Parse("88f706b9-50a1-46be-b201-7d103d360cbe"), "Application Development", "Data"),
                    (Guid.Parse("ced47138-ddb9-43da-aec6-5a0e70e5ead9"), "Application Development", "Data and AI"),
                    (Guid.Parse("988d1d3b-43b0-43b7-adbc-42ee0a27b76c"), "Application Development", "Digital Assurance"),
                    (Guid.Parse("3a61e088-cc07-4eb1-8cef-384457bde283"), "Application Development", "Java"),
                    (Guid.Parse("14de8e1e-1d92-4b2f-b86a-a8ef0db7397b"), "Application Development", "LAMP"),
                    (Guid.Parse("52910d12-a887-4798-ab1d-7dbeafae2f9e"), "Application Development", "Microsoft"),
                    (Guid.Parse("5601b2b4-67e4-470d-9e89-6f302adfd661"), "Application Development", "Mobility"),
                    (Guid.Parse("d5342790-beae-4f69-bf0b-d803be3c1dbd"), "Application Development", "RPA"),
                    (Guid.Parse("3c440cb6-7124-4774-b522-3dfd05683cec"), "BI", "Data and AI"),
                    (Guid.Parse("f65e1ba1-78ad-4ef4-bf5f-20d0f6c80680"), "Business Analyst", "Digital Product Studio"),
                    (Guid.Parse("943dfcea-f5fb-492f-9eea-a5863265c2c6"), "Business Assurance / Functional", "Digital Assurance"),
                    (Guid.Parse("63995ef7-1525-4c55-8fb7-83c025b8171e"), "Business Assurance / Functional", "LAMP"),
                    (Guid.Parse("580494d7-db4e-4ccf-ac21-3d7d47a0e0d7"), "Business Assurance / Functional", "Microsoft"),
                    (Guid.Parse("7dc17b5d-c141-4257-b430-ea837eb79f70"), "Business Assurance / Functional", "RPA"),
                    (Guid.Parse("06781d6a-6536-46d0-91d2-1fd2d992fd28"), "Cloud and Devops", "Microsoft"),
                    (Guid.Parse("c3f96d1e-14f3-4088-abf3-e766f41abfba"), "Data", "Data"),
                    (Guid.Parse("92be13d3-136c-4960-abb8-5f26c478a7e2"), "Data", "Data and AI"),
                    (Guid.Parse("34ed58b0-54bb-4d3f-8ab5-06d893aebb52"), "Data Engineering", "Data"),
                    (Guid.Parse("f7a76000-1ca2-4dee-9b48-f2269257c048"), "Data Engineering", "Data and AI"),
                    (Guid.Parse("c90e1812-53cf-4336-864c-506c2a66f2c9"), "Data Scientist", "Data"),
                    (Guid.Parse("05aa7a2f-bdcb-47b6-bddd-690585ee0a0c"), "Data Scientist", "Data and AI"),
                    (Guid.Parse("ef30931f-0b58-4939-8e4a-f9236be82871"), "Delivery Excellence", "Delivery Excellence"),
                    (Guid.Parse("7f863fd6-99b7-41a5-92f7-d18d6084b1b1"), "Development", "Utilities"),
                    (Guid.Parse("6d7e3850-8929-4e2b-a359-9d83bd6cf6c1"), "Digital Assurance", "Digital Assurance"),
                    (Guid.Parse("52e6a40b-54a0-4196-9c96-fbb77b9782ed"), "Digital Product Studio", "Digital Product Studio"),
                    (Guid.Parse("c2bab55a-6382-48ef-8c91-56f4434722d1"), "Finance", "Finance"),
                    (Guid.Parse("37cd0752-9447-4aed-80e4-4973f857fcf9"), "Functional", "Utilities"),
                    (Guid.Parse("a23736dc-9edf-433d-97de-14ff7e237358"), "IoS", "Mobility"),
                    (Guid.Parse("f2d719da-9b85-4c2d-99a0-4667a418b855"), "ITIS", "Internal IT"),
                    (Guid.Parse("2febcb75-e314-4f63-bcb8-bfbdc019f2e6"), "ITIS", "Managed Services"),
                    (Guid.Parse("54d2cc56-0ac2-4528-aefa-46b045f6e664"), "ITIS", "Sales and Marketing"),
                    (Guid.Parse("7c1ccd1c-0556-466f-b3db-fdd213dc6fb3"), "Management", "Business Excellence"),
                    (Guid.Parse("6e569052-596a-47df-8379-c3ecb41ac2e6"), "Management", "Delivery Excellence"),
                    (Guid.Parse("8e4c7893-9fca-49ac-8c94-d3489fc013ec"), "Management", "Digital Product Studio"),
                    (Guid.Parse("3c0cd1d5-1e54-49ef-8229-7cdb633886d1"), "Management", "Management"),
                    (Guid.Parse("37398092-2f5b-4af9-b4d2-e18d9b452e66"), "Marketing", "Sales and Marketing"),
                    (Guid.Parse("01f85dd2-6a0a-4b7b-8cbf-35433b19fbd0"), "Others", "Delivery Excellence"),
                    (Guid.Parse("6f0ac894-8780-4e3c-80ca-24d76cb90c32"), "PMO", "Business Excellence"),
                    (Guid.Parse("8211b01b-1040-4c1c-9fca-887dad4b2a56"), "Presales", "Sales and Marketing"),
                    (Guid.Parse("e082cdc7-b712-4476-be7f-e9789cd7f250"), "Product Design", "Digital Product Studio"),
                    (Guid.Parse("6771d015-5962-4a9c-b9ed-46cd78918641"), "Product Management", "Digital Product Studio"),
                    (Guid.Parse("725b8002-c1f6-4ce2-a001-52f80607933f"), "Quality Assurance", "Digital Assurance"),
                    (Guid.Parse("6d29085d-a85d-4c16-b09e-dacd3d654188"), "RPA", "RPA"),
                    (Guid.Parse("bbfceaed-a5c9-4608-bb1c-7796bb4ddc2a"), "Sales and Marketing", "Sales and Marketing"),
                    (Guid.Parse("82c765a7-4835-459e-9f26-4c96044d87c2"), "Scrum Master", "Data and AI"),
                    (Guid.Parse("c6ca23c2-c347-4c3f-a83e-87bb612cf907"), "Scrum Master", "Delivery Excellence"),
                    (Guid.Parse("574243bb-2d51-4fd5-8b16-64048db6c05a"), "Talent Acquisition", "HR"),
                    (Guid.Parse("82484b97-7b34-47e6-a68b-ed48420a2a12"), "Talent Management & Operations", "HR"),
                    (Guid.Parse("93f8806b-24a1-4856-9c39-5ed42aa78977"), "Technical Lead", "Microsoft"),
                    (Guid.Parse("e7cf0f2a-e97e-4140-a3c2-c45de4b0e5a0"), "Technology Assurance / Automation", "Digital Assurance"),
                    (Guid.Parse("704f57a3-50c0-4c04-bb6d-bdc235dbe4bc"), "Travel and front tesk", "HR"),
                    (Guid.Parse("5631f807-ccda-479f-a2ff-5b06629b38ee"), "Utility Testing", "Administration"),
                    (Guid.Parse("640e7070-7252-446f-b264-2fcabcb4d171"), "Utility Testing", "Digital Assurance"),
                };

                var subPractices = subPracticeDefs
                    .Where(d => pracMap.ContainsKey(d.PracticeName))
                    .Select(d => new SubPracticeMaster
                    {
                        Id = d.Id,
                        Name = d.Name,
                        PracticeId = pracMap[d.PracticeName],
                        IsActive = true,
                        CreatedOn = now
                    })
                    .ToList();

                if (subPractices.Any())
                {
                    context.Set<SubPracticeMaster>().AddRange(subPractices);
                    await context.SaveChangesAsync();
                }
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

            if (!await context.Clients.AnyAsync())
            {
                context.Clients.Add(new Client
                {
                    Name = "Internal",
                    StatusId = StatusActive,
                    Location = "Internal",
                    CreatedOn = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                });
                await context.SaveChangesAsync();
            }

            // Project seeding removed — no seed data for new schema yet
        }
    }
}
