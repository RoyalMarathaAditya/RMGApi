using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HRMS.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddAllocationStatusId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "StatusId",
                table: "ResourceAllocations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.InsertData(
                table: "StatusMasters",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "IsActive", "IsDeleted", "ModifiedBy", "ModifiedOn", "Name" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000003"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, null, null, "Planned" },
                    { new Guid("10000000-0000-0000-0000-000000000004"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, null, null, "Completed" },
                    { new Guid("10000000-0000-0000-0000-000000000005"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, null, null, "Released" },
                    { new Guid("10000000-0000-0000-0000-000000000006"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, null, null, "Cancelled" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResourceAllocations_StatusId",
                table: "ResourceAllocations",
                column: "StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_ResourceAllocations_StatusMasters_StatusId",
                table: "ResourceAllocations",
                column: "StatusId",
                principalTable: "StatusMasters",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResourceAllocations_StatusMasters_StatusId",
                table: "ResourceAllocations");

            migrationBuilder.DropIndex(
                name: "IX_ResourceAllocations_StatusId",
                table: "ResourceAllocations");

            migrationBuilder.DeleteData(
                table: "StatusMasters",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "StatusMasters",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "StatusMasters",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "StatusMasters",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000006"));

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "ResourceAllocations");
        }
    }
}
