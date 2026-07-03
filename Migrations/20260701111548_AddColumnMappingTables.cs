using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HRMS.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnMappingTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ColumnMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceSystem = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SourceColumn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TargetProperty = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TargetDisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DataType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColumnMappings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ColumnValueMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetProperty = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SourceValue = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TargetValue = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColumnValueMappings", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ColumnMappings",
                columns: new[] { "Id", "CreatedOn", "DataType", "DisplayOrder", "IsActive", "IsRequired", "ModifiedOn", "SourceColumn", "SourceSystem", "TargetDisplayName", "TargetProperty" },
                values: new object[,]
                {
                    { new Guid("a1000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "string", 1, true, true, null, "Employee Code", "PeopleStrong", "Emp ID", "EmployeeCode" },
                    { new Guid("a1000000-0000-0000-0000-000000000002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "string", 2, true, false, null, "Employment Status", "PeopleStrong", "Active", "ActiveStatus" },
                    { new Guid("a1000000-0000-0000-0000-000000000003"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "string", 3, true, true, null, "Employment Type", "PeopleStrong", "FTE/ Consultant", "EmployeeType" },
                    { new Guid("a1000000-0000-0000-0000-000000000004"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "string", 4, true, true, null, "Employee Name", "PeopleStrong", "Full Name", "FullName" },
                    { new Guid("a1000000-0000-0000-0000-000000000005"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "string", 5, true, true, null, "Function", "PeopleStrong", "Practice", "Practice" },
                    { new Guid("a1000000-0000-0000-0000-000000000006"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "string", 6, true, false, null, "Department/Practice", "PeopleStrong", "Sub-practice", "SubPractice" },
                    { new Guid("a1000000-0000-0000-0000-000000000007"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "string", 7, false, false, null, "Organization Unit", "PeopleStrong", "Sub-practice", "SubPractice" },
                    { new Guid("a1000000-0000-0000-0000-000000000008"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "string", 8, true, false, null, "Base office location", "PeopleStrong", "Location", "NVLocation" },
                    { new Guid("a1000000-0000-0000-0000-000000000009"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "string", 9, true, true, null, "Office Email Address", "PeopleStrong", "Email ID", "Email" },
                    { new Guid("a1000000-0000-0000-0000-000000000010"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "datetime", 10, true, true, null, "Date of Joining", "PeopleStrong", "DOJ", "DOJ" },
                    { new Guid("a1000000-0000-0000-0000-000000000011"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "string", 11, true, true, null, "Designation", "PeopleStrong", "Role", "Designation" },
                    { new Guid("a1000000-0000-0000-0000-000000000012"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "string", 12, true, false, null, "L1 Manager", "PeopleStrong", "L1 Manager", "ReportingManager" },
                    { new Guid("a1000000-0000-0000-0000-000000000013"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "string", 13, true, false, null, "Practice Head", "PeopleStrong", "Practice Head", "PracticeHead" },
                    { new Guid("a2000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "string", 1, true, false, null, "Emp Id", "RMG", "Emp ID", "EmployeeCode" },
                    { new Guid("a2000000-0000-0000-0000-000000000002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "string", 2, true, true, null, "Full Name", "RMG", "Full Name", "FullName" },
                    { new Guid("a2000000-0000-0000-0000-000000000003"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "string", 3, true, true, null, "FTE/ Consultant", "RMG", "FTE/ Consultant", "EmployeeType" },
                    { new Guid("a2000000-0000-0000-0000-000000000004"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "string", 4, true, true, null, "Role", "RMG", "Role", "Designation" },
                    { new Guid("a2000000-0000-0000-0000-000000000005"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "string", 5, true, true, null, "OU 4 - Practice", "RMG", "Practice", "Practice" },
                    { new Guid("a2000000-0000-0000-0000-000000000006"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "string", 6, true, false, null, "OU 5 - Sub-practice", "RMG", "Sub-practice", "SubPractice" },
                    { new Guid("a2000000-0000-0000-0000-000000000007"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "string", 7, true, false, null, "Location", "RMG", "Location", "NVLocation" },
                    { new Guid("a2000000-0000-0000-0000-000000000008"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "string", 8, true, false, null, "L1 Manager", "RMG", "L1 Manager", "ReportingManager" },
                    { new Guid("a2000000-0000-0000-0000-000000000009"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "string", 9, true, false, null, "Practice Head", "RMG", "Practice Head", "PracticeHead" },
                    { new Guid("a2000000-0000-0000-0000-000000000010"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "string", 10, true, true, null, "email ID", "RMG", "Email ID", "Email" },
                    { new Guid("a2000000-0000-0000-0000-000000000011"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "string", 11, true, false, null, "Active", "RMG", "Active", "ActiveStatus" },
                    { new Guid("a2000000-0000-0000-0000-000000000012"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "datetime", 12, true, true, null, "DOJ", "RMG", "DOJ", "DOJ" },
                    { new Guid("a2000000-0000-0000-0000-000000000013"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "datetime", 13, true, false, null, "LWD", "RMG", "LWD", "LWD" }
                });

            migrationBuilder.InsertData(
                table: "ColumnValueMappings",
                columns: new[] { "Id", "CreatedOn", "IsActive", "ModifiedOn", "SourceValue", "TargetProperty", "TargetValue" },
                values: new object[,]
                {
                    { new Guid("b1000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, null, "Permanent", "EmployeeType", "FTE" },
                    { new Guid("b1000000-0000-0000-0000-000000000002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, null, "Contract", "EmployeeType", "Consultant" },
                    { new Guid("b1000000-0000-0000-0000-000000000003"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, null, "Contractor", "EmployeeType", "Consultant" },
                    { new Guid("b1000000-0000-0000-0000-000000000004"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, null, "Probation", "EmployeeType", "FTE" },
                    { new Guid("b1000000-0000-0000-0000-000000000005"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, null, "Active", "ActiveStatus", "Active" },
                    { new Guid("b1000000-0000-0000-0000-000000000006"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, null, "Inactive", "ActiveStatus", "Inactive" },
                    { new Guid("b1000000-0000-0000-0000-000000000007"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, null, "Yes", "ActiveStatus", "Active" },
                    { new Guid("b1000000-0000-0000-0000-000000000008"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, null, "No", "ActiveStatus", "Inactive" },
                    { new Guid("b1000000-0000-0000-0000-000000000009"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, null, "Y", "ActiveStatus", "Active" },
                    { new Guid("b1000000-0000-0000-0000-000000000010"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, null, "N", "ActiveStatus", "Inactive" },
                    { new Guid("b1000000-0000-0000-0000-000000000011"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, null, "True", "ActiveStatus", "Active" },
                    { new Guid("b1000000-0000-0000-0000-000000000012"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, null, "False", "ActiveStatus", "Inactive" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ColumnMappings_SourceSystem_SourceColumn",
                table: "ColumnMappings",
                columns: new[] { "SourceSystem", "SourceColumn" },
                unique: true,
                filter: "[IsActive] = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ColumnMappings");

            migrationBuilder.DropTable(
                name: "ColumnValueMappings");
        }
    }
}
