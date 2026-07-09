using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HRMS.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingDashboardRmgMappings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("b1000000-0000-0000-0000-000000000004"),
                column: "DisplayOrder",
                value: 5);

            migrationBuilder.UpdateData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("b1000000-0000-0000-0000-000000000005"),
                column: "DisplayOrder",
                value: 7);

            migrationBuilder.UpdateData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("b1000000-0000-0000-0000-000000000006"),
                column: "DisplayOrder",
                value: 8);

            migrationBuilder.UpdateData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("b1000000-0000-0000-0000-000000000007"),
                column: "DisplayOrder",
                value: 9);

            migrationBuilder.UpdateData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("b1000000-0000-0000-0000-000000000008"),
                column: "DisplayOrder",
                value: 10);

            migrationBuilder.UpdateData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("b1000000-0000-0000-0000-000000000009"),
                column: "DisplayOrder",
                value: 11);

            migrationBuilder.UpdateData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("b1000000-0000-0000-0000-000000000010"),
                column: "DisplayOrder",
                value: 12);

            migrationBuilder.UpdateData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("b1000000-0000-0000-0000-000000000011"),
                column: "DisplayOrder",
                value: 13);

            migrationBuilder.UpdateData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("b1000000-0000-0000-0000-000000000012"),
                column: "DisplayOrder",
                value: 14);

            migrationBuilder.UpdateData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("b1000000-0000-0000-0000-000000000013"),
                column: "DisplayOrder",
                value: 15);

            migrationBuilder.InsertData(
                table: "ColumnMappings",
                columns: new[] { "Id", "CreatedOn", "DataType", "DisplayOrder", "EntityType", "IsActive", "IsRequired", "ModifiedOn", "SourceColumn", "TargetDisplayName", "TargetProperty" },
                values: new object[,]
                {
                    { new Guid("b1000000-0000-0000-0000-000000000014"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "decimal", 4, "resource-allocation", true, false, null, "Total Experience", "Total Experience", "TotalExperience" },
                    { new Guid("b1000000-0000-0000-0000-000000000015"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "string", 6, "resource-allocation", true, false, null, "OU 5 - Sub Practice", "OU 5 - Sub Practice", "SubPractice" },
                    { new Guid("b1000000-0000-0000-0000-000000000016"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "string", 16, "resource-allocation", true, false, null, "Resource Status", "Resource Status", "ResourceStatus" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("b1000000-0000-0000-0000-000000000014"));

            migrationBuilder.DeleteData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("b1000000-0000-0000-0000-000000000015"));

            migrationBuilder.DeleteData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("b1000000-0000-0000-0000-000000000016"));

            migrationBuilder.UpdateData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("b1000000-0000-0000-0000-000000000004"),
                column: "DisplayOrder",
                value: 4);

            migrationBuilder.UpdateData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("b1000000-0000-0000-0000-000000000005"),
                column: "DisplayOrder",
                value: 5);

            migrationBuilder.UpdateData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("b1000000-0000-0000-0000-000000000006"),
                column: "DisplayOrder",
                value: 6);

            migrationBuilder.UpdateData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("b1000000-0000-0000-0000-000000000007"),
                column: "DisplayOrder",
                value: 7);

            migrationBuilder.UpdateData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("b1000000-0000-0000-0000-000000000008"),
                column: "DisplayOrder",
                value: 8);

            migrationBuilder.UpdateData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("b1000000-0000-0000-0000-000000000009"),
                column: "DisplayOrder",
                value: 9);

            migrationBuilder.UpdateData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("b1000000-0000-0000-0000-000000000010"),
                column: "DisplayOrder",
                value: 10);

            migrationBuilder.UpdateData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("b1000000-0000-0000-0000-000000000011"),
                column: "DisplayOrder",
                value: 11);

            migrationBuilder.UpdateData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("b1000000-0000-0000-0000-000000000012"),
                column: "DisplayOrder",
                value: 12);

            migrationBuilder.UpdateData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("b1000000-0000-0000-0000-000000000013"),
                column: "DisplayOrder",
                value: 13);
        }
    }
}
