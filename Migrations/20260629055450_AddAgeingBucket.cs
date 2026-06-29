using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HRMS.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddAgeingBucket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AgeingBucketId",
                table: "ResourceAllocations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AgeingBucketMasters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgeingBucketMasters", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "AgeingBucketMasters",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "DisplayOrder", "IsActive", "IsDeleted", "ModifiedBy", "ModifiedOn", "Name" },
                values: new object[,]
                {
                    { new Guid("d0040001-0000-0000-0000-000000000001"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, false, null, null, "0-30 Days" },
                    { new Guid("d0040001-0000-0000-0000-000000000002"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, false, null, null, "31-60 Days" },
                    { new Guid("d0040001-0000-0000-0000-000000000003"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, true, false, null, null, "61-90 Days" },
                    { new Guid("d0040001-0000-0000-0000-000000000004"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, true, false, null, null, "91-120 Days" },
                    { new Guid("d0040001-0000-0000-0000-000000000005"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 5, true, false, null, null, "120+ Days" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResourceAllocations_AgeingBucketId",
                table: "ResourceAllocations",
                column: "AgeingBucketId");

            migrationBuilder.CreateIndex(
                name: "IX_AgeingBucketMasters_Name",
                table: "AgeingBucketMasters",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ResourceAllocations_AgeingBucketMasters_AgeingBucketId",
                table: "ResourceAllocations",
                column: "AgeingBucketId",
                principalTable: "AgeingBucketMasters",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResourceAllocations_AgeingBucketMasters_AgeingBucketId",
                table: "ResourceAllocations");

            migrationBuilder.DropTable(
                name: "AgeingBucketMasters");

            migrationBuilder.DropIndex(
                name: "IX_ResourceAllocations_AgeingBucketId",
                table: "ResourceAllocations");

            migrationBuilder.DropColumn(
                name: "AgeingBucketId",
                table: "ResourceAllocations");
        }
    }
}
