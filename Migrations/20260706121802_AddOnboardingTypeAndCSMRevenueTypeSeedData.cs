using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HRMS.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddOnboardingTypeAndCSMRevenueTypeSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "CSMRevenueTypes",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "IsActive", "IsDeleted", "ModifiedBy", "ModifiedOn", "Name" },
                values: new object[,]
                {
                    { new Guid("e0000000-0000-0000-0000-000000000001"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, null, null, "Time & Material" },
                    { new Guid("e0000000-0000-0000-0000-000000000002"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, null, null, "Fixed Price" },
                    { new Guid("e0000000-0000-0000-0000-000000000003"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, null, null, "Retainer" },
                    { new Guid("e0000000-0000-0000-0000-000000000004"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, null, null, "License" },
                    { new Guid("e0000000-0000-0000-0000-000000000005"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, null, null, "Maintenance" },
                    { new Guid("e0000000-0000-0000-0000-000000000006"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, null, null, "Corporate" }
                });

            migrationBuilder.InsertData(
                table: "OnboardingTypeMasters",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "IsActive", "IsDeleted", "ModifiedBy", "ModifiedOn", "Name" },
                values: new object[,]
                {
                    { new Guid("d0200000-0000-0000-0000-000000000001"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, null, null, "Direct Join" },
                    { new Guid("d0200000-0000-0000-0000-000000000002"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, null, null, "Internal Transfer" },
                    { new Guid("d0200000-0000-0000-0000-000000000003"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, null, null, "Academy Hire" },
                    { new Guid("d0200000-0000-0000-0000-000000000004"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, null, null, "Lateral Hire" },
                    { new Guid("d0200000-0000-0000-0000-000000000005"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, null, null, "Contract to Hire" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CSMRevenueTypes",
                keyColumn: "Id",
                keyValue: new Guid("e0000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "CSMRevenueTypes",
                keyColumn: "Id",
                keyValue: new Guid("e0000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "CSMRevenueTypes",
                keyColumn: "Id",
                keyValue: new Guid("e0000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "CSMRevenueTypes",
                keyColumn: "Id",
                keyValue: new Guid("e0000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "CSMRevenueTypes",
                keyColumn: "Id",
                keyValue: new Guid("e0000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "CSMRevenueTypes",
                keyColumn: "Id",
                keyValue: new Guid("e0000000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "OnboardingTypeMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0200000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "OnboardingTypeMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0200000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "OnboardingTypeMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0200000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "OnboardingTypeMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0200000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "OnboardingTypeMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0200000-0000-0000-0000-000000000005"));
        }
    }
}
