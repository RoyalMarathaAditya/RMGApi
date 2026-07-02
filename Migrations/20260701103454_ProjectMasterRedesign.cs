using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMS.Api.Migrations
{
    public partial class ProjectMasterRedesign : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Employees_CSMId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Employees_ProjectManagerId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Practices_PracticeId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_PricingTypeMasters_PricingTypeId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_ProjectTypeMasters_ProjectTypeId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_CSMId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_PracticeId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_PricingTypeId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_ProjectManagerId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_ProjectTypeId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "OnboardingStatus",
                table: "ResourceAllocations");

            migrationBuilder.DropColumn(
                name: "CSMId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "PracticeId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "PricingTypeId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ProjectManagerId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ProjectTypeId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Projects");

            migrationBuilder.AddColumn<Guid>(
                name: "CSMRevenueTypeId",
                table: "Projects",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeliveryHead",
                table: "Projects",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProjectManager",
                table: "Projects",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CSMRevenueTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CSMRevenueTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_CSMRevenueTypeId",
                table: "Projects",
                column: "CSMRevenueTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_CSMRevenueTypes_CSMRevenueTypeId",
                table: "Projects",
                column: "CSMRevenueTypeId",
                principalTable: "CSMRevenueTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_CSMRevenueTypes_CSMRevenueTypeId",
                table: "Projects");

            migrationBuilder.DropTable(
                name: "CSMRevenueTypes");

            migrationBuilder.DropIndex(
                name: "IX_Projects_CSMRevenueTypeId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "CSMRevenueTypeId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "DeliveryHead",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ProjectManager",
                table: "Projects");

            migrationBuilder.AddColumn<string>(
                name: "OnboardingStatus",
                table: "ResourceAllocations",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CSMId",
                table: "Projects",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Projects",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PracticeId",
                table: "Projects",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "PricingTypeId",
                table: "Projects",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "ProjectManagerId",
                table: "Projects",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProjectTypeId",
                table: "Projects",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Projects",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Projects_CSMId",
                table: "Projects",
                column: "CSMId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_PracticeId",
                table: "Projects",
                column: "PracticeId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_PricingTypeId",
                table: "Projects",
                column: "PricingTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ProjectManagerId",
                table: "Projects",
                column: "ProjectManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ProjectTypeId",
                table: "Projects",
                column: "ProjectTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Employees_CSMId",
                table: "Projects",
                column: "CSMId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Employees_ProjectManagerId",
                table: "Projects",
                column: "ProjectManagerId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Practices_PracticeId",
                table: "Projects",
                column: "PracticeId",
                principalTable: "Practices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_PricingTypeMasters_PricingTypeId",
                table: "Projects",
                column: "PricingTypeId",
                principalTable: "PricingTypeMasters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_ProjectTypeMasters_ProjectTypeId",
                table: "Projects",
                column: "ProjectTypeId",
                principalTable: "ProjectTypeMasters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
