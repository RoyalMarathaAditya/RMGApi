using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HRMS.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddNewMasterFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BillableDateProbabilityId",
                table: "ResourceAllocations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BillingBucketId",
                table: "ResourceAllocations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CurrentBillingStatusId",
                table: "ResourceAllocations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OnboardingStatusId",
                table: "ResourceAllocations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BillableDateProbabilityMasters",
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
                    table.PrimaryKey("PK_BillableDateProbabilityMasters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BillingBucketMasters",
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
                    table.PrimaryKey("PK_BillingBucketMasters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CurrentBillingStatusMasters",
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
                    table.PrimaryKey("PK_CurrentBillingStatusMasters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OnboardingStatusMasters",
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
                    table.PrimaryKey("PK_OnboardingStatusMasters", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "BillableDateProbabilityMasters",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "DisplayOrder", "IsActive", "IsDeleted", "ModifiedBy", "ModifiedOn", "Name" },
                values: new object[,]
                {
                    { new Guid("d0010000-0000-0000-0000-000000000001"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, false, null, null, "High" },
                    { new Guid("d0010000-0000-0000-0000-000000000002"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, false, null, null, "Medium" },
                    { new Guid("d0010000-0000-0000-0000-000000000003"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, true, false, null, null, "Low" },
                    { new Guid("d0010000-0000-0000-0000-000000000004"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, true, false, null, null, "Not Applicable" }
                });

            migrationBuilder.InsertData(
                table: "BillingBucketMasters",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "DisplayOrder", "IsActive", "IsDeleted", "ModifiedBy", "ModifiedOn", "Name" },
                values: new object[,]
                {
                    { new Guid("d0030000-0000-0000-0000-000000000001"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, false, null, null, "Bucket 1" },
                    { new Guid("d0030000-0000-0000-0000-000000000002"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, false, null, null, "Bucket 2" },
                    { new Guid("d0030000-0000-0000-0000-000000000003"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, true, false, null, null, "Bucket 3" },
                    { new Guid("d0030000-0000-0000-0000-000000000004"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, true, false, null, null, "Bucket 4" }
                });

            migrationBuilder.InsertData(
                table: "CurrentBillingStatusMasters",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "DisplayOrder", "IsActive", "IsDeleted", "ModifiedBy", "ModifiedOn", "Name" },
                values: new object[,]
                {
                    { new Guid("d0020000-0000-0000-0000-000000000001"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, false, null, null, "Billable" },
                    { new Guid("d0020000-0000-0000-0000-000000000002"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, false, null, null, "Non-Billable" },
                    { new Guid("d0020000-0000-0000-0000-000000000003"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, true, false, null, null, "Shadow" },
                    { new Guid("d0020000-0000-0000-0000-000000000004"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, true, false, null, null, "To Be Confirmed" }
                });

            migrationBuilder.InsertData(
                table: "OnboardingStatusMasters",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "DisplayOrder", "IsActive", "IsDeleted", "ModifiedBy", "ModifiedOn", "Name" },
                values: new object[,]
                {
                    { new Guid("d0100000-0000-0000-0000-000000000001"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, false, null, null, "Not Onboarded" },
                    { new Guid("d0100000-0000-0000-0000-000000000002"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, false, null, null, "In Progress" },
                    { new Guid("d0100000-0000-0000-0000-000000000003"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, true, false, null, null, "Onboarded" },
                    { new Guid("d0100000-0000-0000-0000-000000000004"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, true, false, null, null, "Exited" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResourceAllocations_BillableDateProbabilityId",
                table: "ResourceAllocations",
                column: "BillableDateProbabilityId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceAllocations_BillingBucketId",
                table: "ResourceAllocations",
                column: "BillingBucketId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceAllocations_CurrentBillingStatusId",
                table: "ResourceAllocations",
                column: "CurrentBillingStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceAllocations_OnboardingStatusId",
                table: "ResourceAllocations",
                column: "OnboardingStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_BillableDateProbabilityMasters_Name",
                table: "BillableDateProbabilityMasters",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BillingBucketMasters_Name",
                table: "BillingBucketMasters",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CurrentBillingStatusMasters_Name",
                table: "CurrentBillingStatusMasters",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OnboardingStatusMasters_Name",
                table: "OnboardingStatusMasters",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ResourceAllocations_BillableDateProbabilityMasters_BillableDateProbabilityId",
                table: "ResourceAllocations",
                column: "BillableDateProbabilityId",
                principalTable: "BillableDateProbabilityMasters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ResourceAllocations_BillingBucketMasters_BillingBucketId",
                table: "ResourceAllocations",
                column: "BillingBucketId",
                principalTable: "BillingBucketMasters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ResourceAllocations_CurrentBillingStatusMasters_CurrentBillingStatusId",
                table: "ResourceAllocations",
                column: "CurrentBillingStatusId",
                principalTable: "CurrentBillingStatusMasters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ResourceAllocations_OnboardingStatusMasters_OnboardingStatusId",
                table: "ResourceAllocations",
                column: "OnboardingStatusId",
                principalTable: "OnboardingStatusMasters",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResourceAllocations_BillableDateProbabilityMasters_BillableDateProbabilityId",
                table: "ResourceAllocations");

            migrationBuilder.DropForeignKey(
                name: "FK_ResourceAllocations_BillingBucketMasters_BillingBucketId",
                table: "ResourceAllocations");

            migrationBuilder.DropForeignKey(
                name: "FK_ResourceAllocations_CurrentBillingStatusMasters_CurrentBillingStatusId",
                table: "ResourceAllocations");

            migrationBuilder.DropForeignKey(
                name: "FK_ResourceAllocations_OnboardingStatusMasters_OnboardingStatusId",
                table: "ResourceAllocations");

            migrationBuilder.DropTable(
                name: "BillableDateProbabilityMasters");

            migrationBuilder.DropTable(
                name: "BillingBucketMasters");

            migrationBuilder.DropTable(
                name: "CurrentBillingStatusMasters");

            migrationBuilder.DropTable(
                name: "OnboardingStatusMasters");

            migrationBuilder.DropIndex(
                name: "IX_ResourceAllocations_BillableDateProbabilityId",
                table: "ResourceAllocations");

            migrationBuilder.DropIndex(
                name: "IX_ResourceAllocations_BillingBucketId",
                table: "ResourceAllocations");

            migrationBuilder.DropIndex(
                name: "IX_ResourceAllocations_CurrentBillingStatusId",
                table: "ResourceAllocations");

            migrationBuilder.DropIndex(
                name: "IX_ResourceAllocations_OnboardingStatusId",
                table: "ResourceAllocations");

            migrationBuilder.DropColumn(
                name: "BillableDateProbabilityId",
                table: "ResourceAllocations");

            migrationBuilder.DropColumn(
                name: "BillingBucketId",
                table: "ResourceAllocations");

            migrationBuilder.DropColumn(
                name: "CurrentBillingStatusId",
                table: "ResourceAllocations");

            migrationBuilder.DropColumn(
                name: "OnboardingStatusId",
                table: "ResourceAllocations");
        }
    }
}
