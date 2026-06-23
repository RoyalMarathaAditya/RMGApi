using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HRMS.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddDesignationMasterCodeSortOrderSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "DesignationMasters",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "DesignationMasters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "DesignationMasters",
                columns: new[] { "Id", "Code", "CreatedBy", "CreatedOn", "IsActive", "IsDeleted", "ModifiedBy", "ModifiedOn", "Name", "SortOrder" },
                values: new object[,]
                {
                    { new Guid("d0000000-0000-0000-0000-000000000001"), "SE", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, null, null, "Software Engineer", 1 },
                    { new Guid("d0000000-0000-0000-0000-000000000002"), "SSE", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, null, null, "Senior Software Engineer", 2 },
                    { new Guid("d0000000-0000-0000-0000-000000000003"), "LSE", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, null, null, "Lead Software Engineer", 3 },
                    { new Guid("d0000000-0000-0000-0000-000000000004"), "TL", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, null, null, "Technical Lead", 4 },
                    { new Guid("d0000000-0000-0000-0000-000000000005"), "ARCH", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, null, null, "Architect", 5 },
                    { new Guid("d0000000-0000-0000-0000-000000000006"), "ASM", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, null, null, "Associate Manager", 6 },
                    { new Guid("d0000000-0000-0000-0000-000000000007"), "MGR", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, null, null, "Manager", 7 },
                    { new Guid("d0000000-0000-0000-0000-000000000008"), "SM", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, null, null, "Senior Manager", 8 },
                    { new Guid("d0000000-0000-0000-0000-000000000009"), "DM", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, null, null, "Delivery Manager", 9 },
                    { new Guid("d0000000-0000-0000-0000-000000000010"), "PM", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, null, null, "Project Manager", 10 },
                    { new Guid("d0000000-0000-0000-0000-000000000011"), "PGM", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, null, null, "Program Manager", 11 },
                    { new Guid("d0000000-0000-0000-0000-000000000012"), "DIR", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, null, null, "Director", 12 },
                    { new Guid("d0000000-0000-0000-0000-000000000013"), "VP", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, null, null, "Vice President", 13 },
                    { new Guid("d0000000-0000-0000-0000-000000000014"), "QAE", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, null, null, "QA Engineer", 14 },
                    { new Guid("d0000000-0000-0000-0000-000000000015"), "SQAE", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, null, null, "Senior QA Engineer", 15 },
                    { new Guid("d0000000-0000-0000-0000-000000000016"), "QAL", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, null, null, "QA Lead", 16 },
                    { new Guid("d0000000-0000-0000-0000-000000000017"), "BA", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, null, null, "Business Analyst", 17 },
                    { new Guid("d0000000-0000-0000-0000-000000000018"), "SBA", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, null, null, "Senior Business Analyst", 18 },
                    { new Guid("d0000000-0000-0000-0000-000000000019"), "SCRUM", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, null, null, "Scrum Master", 19 },
                    { new Guid("d0000000-0000-0000-0000-000000000020"), "PO", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, null, null, "Product Owner", 20 },
                    { new Guid("d0000000-0000-0000-0000-000000000021"), "UIUX", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, null, null, "UI/UX Designer", 21 },
                    { new Guid("d0000000-0000-0000-0000-000000000022"), "DEVOPS", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, null, null, "DevOps Engineer", 22 },
                    { new Guid("d0000000-0000-0000-0000-000000000023"), "DE", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, null, null, "Data Engineer", 23 },
                    { new Guid("d0000000-0000-0000-0000-000000000024"), "DS", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, null, null, "Data Scientist", 24 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_DesignationMasters_Code",
                table: "DesignationMasters",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL AND [Code] != ''");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DesignationMasters_Code",
                table: "DesignationMasters");

            migrationBuilder.DeleteData(
                table: "DesignationMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "DesignationMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "DesignationMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "DesignationMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "DesignationMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "DesignationMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0000000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "DesignationMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0000000-0000-0000-0000-000000000007"));

            migrationBuilder.DeleteData(
                table: "DesignationMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0000000-0000-0000-0000-000000000008"));

            migrationBuilder.DeleteData(
                table: "DesignationMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0000000-0000-0000-0000-000000000009"));

            migrationBuilder.DeleteData(
                table: "DesignationMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0000000-0000-0000-0000-000000000010"));

            migrationBuilder.DeleteData(
                table: "DesignationMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0000000-0000-0000-0000-000000000011"));

            migrationBuilder.DeleteData(
                table: "DesignationMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0000000-0000-0000-0000-000000000012"));

            migrationBuilder.DeleteData(
                table: "DesignationMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0000000-0000-0000-0000-000000000013"));

            migrationBuilder.DeleteData(
                table: "DesignationMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0000000-0000-0000-0000-000000000014"));

            migrationBuilder.DeleteData(
                table: "DesignationMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0000000-0000-0000-0000-000000000015"));

            migrationBuilder.DeleteData(
                table: "DesignationMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0000000-0000-0000-0000-000000000016"));

            migrationBuilder.DeleteData(
                table: "DesignationMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0000000-0000-0000-0000-000000000017"));

            migrationBuilder.DeleteData(
                table: "DesignationMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0000000-0000-0000-0000-000000000018"));

            migrationBuilder.DeleteData(
                table: "DesignationMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0000000-0000-0000-0000-000000000019"));

            migrationBuilder.DeleteData(
                table: "DesignationMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0000000-0000-0000-0000-000000000020"));

            migrationBuilder.DeleteData(
                table: "DesignationMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0000000-0000-0000-0000-000000000021"));

            migrationBuilder.DeleteData(
                table: "DesignationMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0000000-0000-0000-0000-000000000022"));

            migrationBuilder.DeleteData(
                table: "DesignationMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0000000-0000-0000-0000-000000000023"));

            migrationBuilder.DeleteData(
                table: "DesignationMasters",
                keyColumn: "Id",
                keyValue: new Guid("d0000000-0000-0000-0000-000000000024"));

            migrationBuilder.DropColumn(
                name: "Code",
                table: "DesignationMasters");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "DesignationMasters");
        }
    }
}
