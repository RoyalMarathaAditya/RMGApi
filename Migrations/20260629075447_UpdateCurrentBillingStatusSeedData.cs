using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HRMS.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCurrentBillingStatusSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "CurrentBillingStatusMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0020000-0000-0000-0000-000000000001"),
                column: "Name",
                value: "Available Pool");

            migrationBuilder.UpdateData(
                table: "CurrentBillingStatusMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0020000-0000-0000-0000-000000000002"),
                column: "Name",
                value: "Billable");

            migrationBuilder.UpdateData(
                table: "CurrentBillingStatusMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0020000-0000-0000-0000-000000000003"),
                column: "Name",
                value: "Billable - Cost Covered");

            migrationBuilder.UpdateData(
                table: "CurrentBillingStatusMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0020000-0000-0000-0000-000000000004"),
                column: "Name",
                value: "Buffer");

            migrationBuilder.InsertData(
                table: "CurrentBillingStatusMasters",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "DisplayOrder", "IsActive", "IsDeleted", "ModifiedBy", "ModifiedOn", "Name" },
                values: new object[,]
                {
                    { new Guid("d0020000-0000-0000-0000-000000000005"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 5, true, false, null, null, "Confirmed" },
                    { new Guid("d0020000-0000-0000-0000-000000000006"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 6, true, false, null, null, "Core Team" },
                    { new Guid("d0020000-0000-0000-0000-000000000007"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 7, true, false, null, null, "Corporate" },
                    { new Guid("d0020000-0000-0000-0000-000000000008"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 8, true, false, null, null, "Long Leave" },
                    { new Guid("d0020000-0000-0000-0000-000000000009"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 9, true, false, null, null, "Most Likely Billable" },
                    { new Guid("d0020000-0000-0000-0000-00000000000a"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 10, true, false, null, null, "To Be Optimised / Exit / PIP" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CurrentBillingStatusMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0020000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "CurrentBillingStatusMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0020000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "CurrentBillingStatusMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0020000-0000-0000-0000-000000000007"));

            migrationBuilder.DeleteData(
                table: "CurrentBillingStatusMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0020000-0000-0000-0000-000000000008"));

            migrationBuilder.DeleteData(
                table: "CurrentBillingStatusMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0020000-0000-0000-0000-000000000009"));

            migrationBuilder.DeleteData(
                table: "CurrentBillingStatusMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0020000-0000-0000-0000-00000000000a"));

            migrationBuilder.UpdateData(
                table: "CurrentBillingStatusMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0020000-0000-0000-0000-000000000001"),
                column: "Name",
                value: "Billable");

            migrationBuilder.UpdateData(
                table: "CurrentBillingStatusMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0020000-0000-0000-0000-000000000002"),
                column: "Name",
                value: "Non-Billable");

            migrationBuilder.UpdateData(
                table: "CurrentBillingStatusMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0020000-0000-0000-0000-000000000003"),
                column: "Name",
                value: "Shadow");

            migrationBuilder.UpdateData(
                table: "CurrentBillingStatusMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0020000-0000-0000-0000-000000000004"),
                column: "Name",
                value: "To Be Confirmed");
        }
    }
}
