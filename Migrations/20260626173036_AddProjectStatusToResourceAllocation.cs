using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HRMS.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectStatusToResourceAllocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProjectStatusId",
                table: "ResourceAllocations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProjectStatusMasters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectStatusMasters", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ProjectStatusMasters",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "DisplayOrder", "IsActive", "IsDeleted", "ModifiedBy", "ModifiedOn", "Name" },
                values: new object[,]
                {
                    { new Guid("a0000000-0000-0000-0000-000000000001"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, false, null, null, "Utilized" },
                    { new Guid("a0000000-0000-0000-0000-000000000002"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, false, null, null, "Billable" },
                    { new Guid("a0000000-0000-0000-0000-000000000003"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, true, false, null, null, "Long Leave" },
                    { new Guid("a0000000-0000-0000-0000-000000000004"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, true, false, null, null, "Management" },
                    { new Guid("a0000000-0000-0000-0000-000000000005"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 5, true, false, null, null, "Onboarding" },
                    { new Guid("a0000000-0000-0000-0000-000000000006"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 6, true, false, null, null, "Onboarding 1$" },
                    { new Guid("a0000000-0000-0000-0000-000000000007"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 7, true, false, null, null, "Scoped" },
                    { new Guid("a0000000-0000-0000-0000-000000000008"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 8, true, false, null, null, "To be Scoped" },
                    { new Guid("a0000000-0000-0000-0000-000000000009"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 9, true, false, null, null, "1$ Utilized" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResourceAllocations_ProjectStatusId",
                table: "ResourceAllocations",
                column: "ProjectStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectStatusMasters_Name",
                table: "ProjectStatusMasters",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ResourceAllocations_ProjectStatusMasters_ProjectStatusId",
                table: "ResourceAllocations",
                column: "ProjectStatusId",
                principalTable: "ProjectStatusMasters",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResourceAllocations_ProjectStatusMasters_ProjectStatusId",
                table: "ResourceAllocations");

            migrationBuilder.DropTable(
                name: "ProjectStatusMasters");

            migrationBuilder.DropIndex(
                name: "IX_ResourceAllocations_ProjectStatusId",
                table: "ResourceAllocations");

            migrationBuilder.DropColumn(
                name: "ProjectStatusId",
                table: "ResourceAllocations");
        }
    }
}
