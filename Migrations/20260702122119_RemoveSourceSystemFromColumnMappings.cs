using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HRMS.Api.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSourceSystemFromColumnMappings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("a2000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("a2000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("a2000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("a2000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("a2000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("a2000000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("a2000000-0000-0000-0000-000000000007"));

            migrationBuilder.DeleteData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("a2000000-0000-0000-0000-000000000008"));

            migrationBuilder.DeleteData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("a2000000-0000-0000-0000-000000000009"));

            migrationBuilder.DeleteData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("a2000000-0000-0000-0000-000000000010"));

            migrationBuilder.DeleteData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("a2000000-0000-0000-0000-000000000011"));

            migrationBuilder.DeleteData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("a2000000-0000-0000-0000-000000000012"));

            migrationBuilder.DeleteData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("a2000000-0000-0000-0000-000000000013"));

            migrationBuilder.DropIndex(
                name: "IX_ColumnMappings_SourceSystem_SourceColumn",
                table: "ColumnMappings");

            migrationBuilder.DropColumn(
                name: "SourceSystem",
                table: "ColumnMappings");

            migrationBuilder.CreateIndex(
                name: "IX_ColumnMappings_SourceColumn",
                table: "ColumnMappings",
                column: "SourceColumn",
                unique: true,
                filter: "[IsActive] = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ColumnMappings_SourceColumn",
                table: "ColumnMappings");

            migrationBuilder.AddColumn<string>(
                name: "SourceSystem",
                table: "ColumnMappings",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("UPDATE [ColumnMappings] SET [SourceSystem] = 'PeopleStrong' WHERE [Id] LIKE 'A100%'");

            migrationBuilder.InsertData(
                table: "ColumnMappings",
                columns: new[] { "Id", "CreatedOn", "DataType", "DisplayOrder", "IsActive", "IsRequired", "ModifiedOn", "SourceColumn", "SourceSystem", "TargetDisplayName", "TargetProperty" },
                values: new object[,]
                {
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

            migrationBuilder.CreateIndex(
                name: "IX_ColumnMappings_SourceSystem_SourceColumn",
                table: "ColumnMappings",
                columns: new[] { "SourceSystem", "SourceColumn" },
                unique: true,
                filter: "[IsActive] = 1");
        }
    }
}
