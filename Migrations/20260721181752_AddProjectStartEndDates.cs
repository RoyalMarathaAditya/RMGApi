using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMS.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectStartEndDates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ProjectEndDate",
                table: "Projects",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ProjectStartDate",
                table: "Projects",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ProjectEndDate", "ProjectStartDate" },
                values: new object[] { new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ProjectEndDate", "ProjectStartDate" },
                values: new object[] { new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ProjectEndDate", "ProjectStartDate" },
                values: new object[] { new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ProjectEndDate", "ProjectStartDate" },
                values: new object[] { new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ProjectEndDate", "ProjectStartDate" },
                values: new object[] { new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "ProjectEndDate", "ProjectStartDate" },
                values: new object[] { new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "ProjectEndDate", "ProjectStartDate" },
                values: new object[] { new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "ProjectEndDate", "ProjectStartDate" },
                values: new object[] { new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "ProjectEndDate", "ProjectStartDate" },
                values: new object[] { new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "ProjectEndDate", "ProjectStartDate" },
                values: new object[] { new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "ProjectEndDate", "ProjectStartDate" },
                values: new object[] { new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "ProjectEndDate", "ProjectStartDate" },
                values: new object[] { new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "ProjectEndDate", "ProjectStartDate" },
                values: new object[] { new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "ProjectEndDate", "ProjectStartDate" },
                values: new object[] { new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "ProjectEndDate", "ProjectStartDate" },
                values: new object[] { new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "ProjectEndDate", "ProjectStartDate" },
                values: new object[] { new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "ProjectEndDate", "ProjectStartDate" },
                values: new object[] { new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "ProjectEndDate", "ProjectStartDate" },
                values: new object[] { new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "ProjectEndDate", "ProjectStartDate" },
                values: new object[] { new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "ProjectEndDate", "ProjectStartDate" },
                values: new object[] { new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "ProjectEndDate", "ProjectStartDate" },
                values: new object[] { new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "ProjectEndDate", "ProjectStartDate" },
                values: new object[] { new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "ProjectEndDate", "ProjectStartDate" },
                values: new object[] { new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "ProjectEndDate", "ProjectStartDate" },
                values: new object[] { new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "ProjectEndDate", "ProjectStartDate" },
                values: new object[] { new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "ProjectEndDate", "ProjectStartDate" },
                values: new object[] { new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "ProjectEndDate", "ProjectStartDate" },
                values: new object[] { new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "ProjectEndDate", "ProjectStartDate" },
                values: new object[] { new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "ProjectEndDate", "ProjectStartDate" },
                values: new object[] { new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "ProjectEndDate", "ProjectStartDate" },
                values: new object[] { new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 31,
                columns: new[] { "ProjectEndDate", "ProjectStartDate" },
                values: new object[] { new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 32,
                columns: new[] { "ProjectEndDate", "ProjectStartDate" },
                values: new object[] { new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 33,
                columns: new[] { "ProjectEndDate", "ProjectStartDate" },
                values: new object[] { new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 34,
                columns: new[] { "ProjectEndDate", "ProjectStartDate" },
                values: new object[] { new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 35,
                columns: new[] { "ProjectEndDate", "ProjectStartDate" },
                values: new object[] { new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 36,
                columns: new[] { "ProjectEndDate", "ProjectStartDate" },
                values: new object[] { new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 37,
                columns: new[] { "ProjectEndDate", "ProjectStartDate" },
                values: new object[] { new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 38,
                columns: new[] { "ProjectEndDate", "ProjectStartDate" },
                values: new object[] { new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 39,
                columns: new[] { "ProjectEndDate", "ProjectStartDate" },
                values: new object[] { new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 40,
                columns: new[] { "ProjectEndDate", "ProjectStartDate" },
                values: new object[] { new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 41,
                columns: new[] { "ProjectEndDate", "ProjectStartDate" },
                values: new object[] { new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 42,
                columns: new[] { "ProjectEndDate", "ProjectStartDate" },
                values: new object[] { new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 43,
                columns: new[] { "ProjectEndDate", "ProjectStartDate" },
                values: new object[] { new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 44,
                columns: new[] { "ProjectEndDate", "ProjectStartDate" },
                values: new object[] { new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 45,
                columns: new[] { "ProjectEndDate", "ProjectStartDate" },
                values: new object[] { new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 46,
                columns: new[] { "ProjectEndDate", "ProjectStartDate" },
                values: new object[] { new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 47,
                columns: new[] { "ProjectEndDate", "ProjectStartDate" },
                values: new object[] { new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 48,
                columns: new[] { "ProjectEndDate", "ProjectStartDate" },
                values: new object[] { new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 49,
                columns: new[] { "ProjectEndDate", "ProjectStartDate" },
                values: new object[] { new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 50,
                columns: new[] { "ProjectEndDate", "ProjectStartDate" },
                values: new object[] { new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 51,
                columns: new[] { "ProjectEndDate", "ProjectStartDate" },
                values: new object[] { new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 52,
                columns: new[] { "ProjectEndDate", "ProjectStartDate" },
                values: new object[] { new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 53,
                columns: new[] { "ProjectEndDate", "ProjectStartDate" },
                values: new object[] { new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 54,
                columns: new[] { "ProjectEndDate", "ProjectStartDate" },
                values: new object[] { new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 55,
                columns: new[] { "ProjectEndDate", "ProjectStartDate" },
                values: new object[] { new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 56,
                columns: new[] { "ProjectEndDate", "ProjectStartDate" },
                values: new object[] { new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 57,
                columns: new[] { "ProjectEndDate", "ProjectStartDate" },
                values: new object[] { new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 58,
                columns: new[] { "ProjectEndDate", "ProjectStartDate" },
                values: new object[] { new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProjectEndDate",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ProjectStartDate",
                table: "Projects");
        }
    }
}
