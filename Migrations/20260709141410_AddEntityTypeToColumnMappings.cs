using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HRMS.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddEntityTypeToColumnMappings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ColumnMappings_SourceColumn",
                table: "ColumnMappings");

            migrationBuilder.AddColumn<string>(
                name: "EntityType",
                table: "ColumnMappings",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "employee-import");

            migrationBuilder.UpdateData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("a1000000-0000-0000-0000-000000000001"),
                column: "EntityType",
                value: "employee-import");

            migrationBuilder.UpdateData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("a1000000-0000-0000-0000-000000000002"),
                column: "EntityType",
                value: "employee-import");

            migrationBuilder.UpdateData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("a1000000-0000-0000-0000-000000000003"),
                column: "EntityType",
                value: "employee-import");

            migrationBuilder.UpdateData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("a1000000-0000-0000-0000-000000000004"),
                column: "EntityType",
                value: "employee-import");

            migrationBuilder.UpdateData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("a1000000-0000-0000-0000-000000000005"),
                column: "EntityType",
                value: "employee-import");

            migrationBuilder.UpdateData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("a1000000-0000-0000-0000-000000000006"),
                column: "EntityType",
                value: "employee-import");

            migrationBuilder.UpdateData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("a1000000-0000-0000-0000-000000000007"),
                column: "EntityType",
                value: "employee-import");

            migrationBuilder.UpdateData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("a1000000-0000-0000-0000-000000000008"),
                column: "EntityType",
                value: "employee-import");

            migrationBuilder.UpdateData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("a1000000-0000-0000-0000-000000000009"),
                column: "EntityType",
                value: "employee-import");

            migrationBuilder.UpdateData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("a1000000-0000-0000-0000-000000000010"),
                column: "EntityType",
                value: "employee-import");

            migrationBuilder.UpdateData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("a1000000-0000-0000-0000-000000000011"),
                column: "EntityType",
                value: "employee-import");

            migrationBuilder.UpdateData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("a1000000-0000-0000-0000-000000000012"),
                column: "EntityType",
                value: "employee-import");

            migrationBuilder.UpdateData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("a1000000-0000-0000-0000-000000000013"),
                column: "EntityType",
                value: "employee-import");

            migrationBuilder.UpdateData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("a1000000-0000-0000-0000-000000000014"),
                column: "EntityType",
                value: "employee-import");

            migrationBuilder.InsertData(
                table: "ColumnMappings",
                columns: new[] { "Id", "CreatedOn", "DataType", "DisplayOrder", "EntityType", "IsActive", "IsRequired", "ModifiedOn", "SourceColumn", "TargetDisplayName", "TargetProperty" },
                values: new object[,]
                {
                    { new Guid("b1000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "string", 1, "resource-allocation", true, false, null, "Full Name", "Full Name", "EmployeeName" },
                    { new Guid("b1000000-0000-0000-0000-000000000002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "string", 2, "resource-allocation", true, false, null, "Emp Id", "Emp Id", "EmployeeCode" },
                    { new Guid("b1000000-0000-0000-0000-000000000003"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "string", 3, "resource-allocation", true, false, null, "Role", "Role", "Designation" },
                    { new Guid("b1000000-0000-0000-0000-000000000004"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "string", 4, "resource-allocation", true, false, null, "OU 4 - Practice", "OU 4 - Practice", "Practice" },
                    { new Guid("b1000000-0000-0000-0000-000000000005"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "string", 5, "resource-allocation", true, false, null, "Project", "Project", "ProjectName" },
                    { new Guid("b1000000-0000-0000-0000-000000000006"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "datetime", 6, "resource-allocation", true, false, null, "Start Date", "Start Date", "StartDate" },
                    { new Guid("b1000000-0000-0000-0000-000000000007"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "datetime", 7, "resource-allocation", true, false, null, "End Date", "End Date", "EndDate" },
                    { new Guid("b1000000-0000-0000-0000-000000000008"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "decimal", 8, "resource-allocation", true, false, null, "Allocation %", "Allocation %", "AllocationPercentage" },
                    { new Guid("b1000000-0000-0000-0000-000000000009"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "string", 9, "resource-allocation", true, false, null, "Status", "Status", "AllocationStatus" },
                    { new Guid("b1000000-0000-0000-0000-000000000010"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "string", 10, "resource-allocation", true, false, null, "Billable Status", "Billable Status", "BillableStatus" },
                    { new Guid("b1000000-0000-0000-0000-000000000011"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "string", 11, "resource-allocation", true, false, null, "Allocation Type", "Allocation Type", "AllocationType" },
                    { new Guid("b1000000-0000-0000-0000-000000000012"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "decimal", 12, "resource-allocation", true, false, null, "Total Allocated", "Total Allocated", "TotalAllocated" },
                    { new Guid("b1000000-0000-0000-0000-000000000013"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "decimal", 13, "resource-allocation", true, false, null, "Available", "Available", "AvailableCapacity" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ColumnMappings_EntityType_SourceColumn",
                table: "ColumnMappings",
                columns: new[] { "EntityType", "SourceColumn" },
                unique: true,
                filter: "[IsActive] = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ColumnMappings_EntityType_SourceColumn",
                table: "ColumnMappings");

            migrationBuilder.DeleteData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("b1000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("b1000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("b1000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("b1000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("b1000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("b1000000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("b1000000-0000-0000-0000-000000000007"));

            migrationBuilder.DeleteData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("b1000000-0000-0000-0000-000000000008"));

            migrationBuilder.DeleteData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("b1000000-0000-0000-0000-000000000009"));

            migrationBuilder.DeleteData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("b1000000-0000-0000-0000-000000000010"));

            migrationBuilder.DeleteData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("b1000000-0000-0000-0000-000000000011"));

            migrationBuilder.DeleteData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("b1000000-0000-0000-0000-000000000012"));

            migrationBuilder.DeleteData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("b1000000-0000-0000-0000-000000000013"));

            migrationBuilder.DropColumn(
                name: "EntityType",
                table: "ColumnMappings");

            migrationBuilder.CreateIndex(
                name: "IX_ColumnMappings_SourceColumn",
                table: "ColumnMappings",
                column: "SourceColumn",
                unique: true,
                filter: "[IsActive] = 1");
        }
    }
}
