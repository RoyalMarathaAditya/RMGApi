using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMS.Api.Migrations
{
    /// <inheritdoc />
    public partial class ChangeOnboardingStatusToString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResourceAllocations_OnboardingStatusMasters_OnboardingStatusId",
                table: "ResourceAllocations");

            migrationBuilder.DropIndex(
                name: "IX_ResourceAllocations_OnboardingStatusId",
                table: "ResourceAllocations");

            migrationBuilder.DropColumn(
                name: "OnboardingStatusId",
                table: "ResourceAllocations");

            migrationBuilder.AddColumn<string>(
                name: "OnboardingStatus",
                table: "ResourceAllocations",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OnboardingStatus",
                table: "ResourceAllocations");

            migrationBuilder.AddColumn<Guid>(
                name: "OnboardingStatusId",
                table: "ResourceAllocations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ResourceAllocations_OnboardingStatusId",
                table: "ResourceAllocations",
                column: "OnboardingStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_ResourceAllocations_OnboardingStatusMasters_OnboardingStatusId",
                table: "ResourceAllocations",
                column: "OnboardingStatusId",
                principalTable: "OnboardingStatusMasters",
                principalColumn: "Id");
        }
    }
}
