using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HRMS.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBillableDateProbabilitySeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "BillableDateProbabilityMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0010000-0000-0000-0000-000000000001"),
                column: "Name",
                value: "Billable");

            migrationBuilder.UpdateData(
                table: "BillableDateProbabilityMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0010000-0000-0000-0000-000000000002"),
                column: "Name",
                value: "Billable - Cost covered");

            migrationBuilder.UpdateData(
                table: "BillableDateProbabilityMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0010000-0000-0000-0000-000000000003"),
                column: "Name",
                value: "Confirmed");

            migrationBuilder.UpdateData(
                table: "BillableDateProbabilityMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0010000-0000-0000-0000-000000000004"),
                column: "Name",
                value: "Core Team");

            migrationBuilder.InsertData(
                table: "BillableDateProbabilityMasters",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "DisplayOrder", "IsActive", "IsDeleted", "ModifiedBy", "ModifiedOn", "Name" },
                values: new object[,]
                {
                    { new Guid("d0010000-0000-0000-0000-000000000005"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 5, true, false, null, null, "Corporate" },
                    { new Guid("d0010000-0000-0000-0000-000000000006"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 6, true, false, null, null, "Long Leave" },
                    { new Guid("d0010000-0000-0000-0000-000000000007"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 7, true, false, null, null, "Most Likely Billable" },
                    { new Guid("d0010000-0000-0000-0000-000000000008"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 8, true, false, null, null, "Non-Billable" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "BillableDateProbabilityMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0010000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "BillableDateProbabilityMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0010000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "BillableDateProbabilityMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0010000-0000-0000-0000-000000000007"));

            migrationBuilder.DeleteData(
                table: "BillableDateProbabilityMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0010000-0000-0000-0000-000000000008"));

            migrationBuilder.UpdateData(
                table: "BillableDateProbabilityMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0010000-0000-0000-0000-000000000001"),
                column: "Name",
                value: "High");

            migrationBuilder.UpdateData(
                table: "BillableDateProbabilityMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0010000-0000-0000-0000-000000000002"),
                column: "Name",
                value: "Medium");

            migrationBuilder.UpdateData(
                table: "BillableDateProbabilityMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0010000-0000-0000-0000-000000000003"),
                column: "Name",
                value: "Low");

            migrationBuilder.UpdateData(
                table: "BillableDateProbabilityMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0010000-0000-0000-0000-000000000004"),
                column: "Name",
                value: "Not Applicable");
        }
    }
}
