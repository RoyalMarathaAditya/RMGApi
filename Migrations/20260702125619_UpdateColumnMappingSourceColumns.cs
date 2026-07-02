using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMS.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateColumnMappingSourceColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("a1000000-0000-0000-0000-000000000001"),
                columns: new[] { "SourceColumn", "TargetDisplayName" },
                values: new object[] { "Emp Id", "Emp Id" });

            migrationBuilder.UpdateData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("a1000000-0000-0000-0000-000000000002"),
                columns: new[] { "IsRequired", "SourceColumn", "TargetDisplayName", "TargetProperty" },
                values: new object[] { true, "Full Name", "Full Name", "FullName" });

            migrationBuilder.UpdateData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("a1000000-0000-0000-0000-000000000003"),
                column: "SourceColumn",
                value: "FTE/ Consultant");

            migrationBuilder.UpdateData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("a1000000-0000-0000-0000-000000000004"),
                columns: new[] { "SourceColumn", "TargetDisplayName", "TargetProperty" },
                values: new object[] { "Role", "Role", "Designation" });

            migrationBuilder.UpdateData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("a1000000-0000-0000-0000-000000000005"),
                columns: new[] { "SourceColumn", "TargetDisplayName" },
                values: new object[] { "OU 4 - Practice", "OU 4 - Practice" });

            migrationBuilder.UpdateData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("a1000000-0000-0000-0000-000000000006"),
                columns: new[] { "SourceColumn", "TargetDisplayName" },
                values: new object[] { "OU 5 - Sub-practice", "OU 5 - Sub-practice" });

            // Free up conflicting SourceColumn values first so the unique index does not block
            migrationBuilder.UpdateData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("a1000000-0000-0000-0000-000000000008"),
                column: "SourceColumn",
                value: "Location");

            migrationBuilder.UpdateData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("a1000000-0000-0000-0000-000000000012"),
                columns: new[] { "SourceColumn", "TargetDisplayName", "TargetProperty" },
                values: new object[] { "Active", "Active", "ActiveStatus" });

            migrationBuilder.UpdateData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("a1000000-0000-0000-0000-000000000013"),
                columns: new[] { "DataType", "IsRequired", "SourceColumn", "TargetDisplayName", "TargetProperty" },
                values: new object[] { "datetime", true, "DOJ", "DOJ", "DOJ" });

            migrationBuilder.UpdateData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("a1000000-0000-0000-0000-000000000009"),
                columns: new[] { "IsRequired", "SourceColumn", "TargetDisplayName", "TargetProperty" },
                values: new object[] { false, "L1 Manager", "L1 Manager", "ReportingManager" });

            migrationBuilder.UpdateData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("a1000000-0000-0000-0000-000000000010"),
                columns: new[] { "DataType", "IsRequired", "SourceColumn", "TargetDisplayName", "TargetProperty" },
                values: new object[] { "string", false, "Practice Head", "Practice Head", "PracticeHead" });

            migrationBuilder.UpdateData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("a1000000-0000-0000-0000-000000000011"),
                columns: new[] { "SourceColumn", "TargetDisplayName", "TargetProperty" },
                values: new object[] { "email ID", "Email ID", "Email" });

            migrationBuilder.InsertData(
                table: "ColumnMappings",
                columns: new[] { "Id", "CreatedOn", "DataType", "DisplayOrder", "IsActive", "IsRequired", "ModifiedOn", "SourceColumn", "TargetDisplayName", "TargetProperty" },
                values: new object[] { new Guid("a1000000-0000-0000-0000-000000000014"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "datetime", 14, true, false, null, "LWD", "LWD", "LWD" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("a1000000-0000-0000-0000-000000000014"));

            migrationBuilder.UpdateData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("a1000000-0000-0000-0000-000000000001"),
                columns: new[] { "SourceColumn", "TargetDisplayName" },
                values: new object[] { "Employee Code", "Emp ID" });

            migrationBuilder.UpdateData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("a1000000-0000-0000-0000-000000000002"),
                columns: new[] { "IsRequired", "SourceColumn", "TargetDisplayName", "TargetProperty" },
                values: new object[] { false, "Employment Status", "Active", "ActiveStatus" });

            migrationBuilder.UpdateData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("a1000000-0000-0000-0000-000000000003"),
                column: "SourceColumn",
                value: "Employment Type");

            migrationBuilder.UpdateData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("a1000000-0000-0000-0000-000000000004"),
                columns: new[] { "SourceColumn", "TargetDisplayName", "TargetProperty" },
                values: new object[] { "Employee Name", "Full Name", "FullName" });

            migrationBuilder.UpdateData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("a1000000-0000-0000-0000-000000000005"),
                columns: new[] { "SourceColumn", "TargetDisplayName" },
                values: new object[] { "Function", "Practice" });

            migrationBuilder.UpdateData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("a1000000-0000-0000-0000-000000000006"),
                columns: new[] { "SourceColumn", "TargetDisplayName" },
                values: new object[] { "Department/Practice", "Sub-practice" });

            migrationBuilder.UpdateData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("a1000000-0000-0000-0000-000000000008"),
                column: "SourceColumn",
                value: "Base office location");

            migrationBuilder.UpdateData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("a1000000-0000-0000-0000-000000000009"),
                columns: new[] { "IsRequired", "SourceColumn", "TargetDisplayName", "TargetProperty" },
                values: new object[] { true, "Office Email Address", "Email ID", "Email" });

            migrationBuilder.UpdateData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("a1000000-0000-0000-0000-000000000010"),
                columns: new[] { "DataType", "IsRequired", "SourceColumn", "TargetDisplayName", "TargetProperty" },
                values: new object[] { "datetime", true, "Date of Joining", "DOJ", "DOJ" });

            migrationBuilder.UpdateData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("a1000000-0000-0000-0000-000000000011"),
                columns: new[] { "SourceColumn", "TargetDisplayName", "TargetProperty" },
                values: new object[] { "Designation", "Role", "Designation" });

            migrationBuilder.UpdateData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("a1000000-0000-0000-0000-000000000012"),
                columns: new[] { "SourceColumn", "TargetDisplayName", "TargetProperty" },
                values: new object[] { "L1 Manager", "L1 Manager", "ReportingManager" });

            migrationBuilder.UpdateData(
                table: "ColumnMappings",
                keyColumn: "Id",
                keyValue: new Guid("a1000000-0000-0000-0000-000000000013"),
                columns: new[] { "DataType", "IsRequired", "SourceColumn", "TargetDisplayName", "TargetProperty" },
                values: new object[] { "string", false, "Practice Head", "Practice Head", "PracticeHead" });
        }
    }
}
