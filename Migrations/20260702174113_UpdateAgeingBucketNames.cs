using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMS.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAgeingBucketNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AgeingBucketMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0040001-0000-0000-0000-000000000005"));

            migrationBuilder.UpdateData(
                table: "AgeingBucketMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0040001-0000-0000-0000-000000000001"),
                column: "Name",
                value: "< 1 month");

            migrationBuilder.UpdateData(
                table: "AgeingBucketMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0040001-0000-0000-0000-000000000002"),
                column: "Name",
                value: "1-3 months");

            migrationBuilder.UpdateData(
                table: "AgeingBucketMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0040001-0000-0000-0000-000000000003"),
                column: "Name",
                value: "3 to 6 months");

            migrationBuilder.UpdateData(
                table: "AgeingBucketMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0040001-0000-0000-0000-000000000004"),
                column: "Name",
                value: "> 6 months");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AgeingBucketMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0040001-0000-0000-0000-000000000001"),
                column: "Name",
                value: "0-30 Days");

            migrationBuilder.UpdateData(
                table: "AgeingBucketMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0040001-0000-0000-0000-000000000002"),
                column: "Name",
                value: "31-60 Days");

            migrationBuilder.UpdateData(
                table: "AgeingBucketMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0040001-0000-0000-0000-000000000003"),
                column: "Name",
                value: "61-90 Days");

            migrationBuilder.UpdateData(
                table: "AgeingBucketMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0040001-0000-0000-0000-000000000004"),
                column: "Name",
                value: "91-120 Days");

            migrationBuilder.InsertData(
                table: "AgeingBucketMasters",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "DisplayOrder", "IsActive", "IsDeleted", "ModifiedBy", "ModifiedOn", "Name" },
                values: new object[] { new Guid("d0040001-0000-0000-0000-000000000005"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 5, true, false, null, null, "120+ Days" });
        }
    }
}
