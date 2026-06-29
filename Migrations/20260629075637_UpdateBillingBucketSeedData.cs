using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMS.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBillingBucketSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "BillingBucketMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0030000-0000-0000-0000-000000000004"));

            migrationBuilder.UpdateData(
                table: "BillingBucketMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0030000-0000-0000-0000-000000000001"),
                column: "Name",
                value: "Billing Team");

            migrationBuilder.UpdateData(
                table: "BillingBucketMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0030000-0000-0000-0000-000000000002"),
                column: "Name",
                value: "Core Team");

            migrationBuilder.UpdateData(
                table: "BillingBucketMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0030000-0000-0000-0000-000000000003"),
                column: "Name",
                value: "Corporate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "BillingBucketMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0030000-0000-0000-0000-000000000001"),
                column: "Name",
                value: "Bucket 1");

            migrationBuilder.UpdateData(
                table: "BillingBucketMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0030000-0000-0000-0000-000000000002"),
                column: "Name",
                value: "Bucket 2");

            migrationBuilder.UpdateData(
                table: "BillingBucketMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0030000-0000-0000-0000-000000000003"),
                column: "Name",
                value: "Bucket 3");

            migrationBuilder.InsertData(
                table: "BillingBucketMasters",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "DisplayOrder", "IsActive", "IsDeleted", "ModifiedBy", "ModifiedOn", "Name" },
                values: new object[] { new Guid("d0030000-0000-0000-0000-000000000004"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, true, false, null, null, "Bucket 4" });
        }
    }
}
