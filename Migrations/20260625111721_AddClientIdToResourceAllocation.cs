using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMS.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddClientIdToResourceAllocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClientId",
                table: "ResourceAllocations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ResourceAllocations_ClientId",
                table: "ResourceAllocations",
                column: "ClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_ResourceAllocations_Clients_ClientId",
                table: "ResourceAllocations",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResourceAllocations_Clients_ClientId",
                table: "ResourceAllocations");

            migrationBuilder.DropIndex(
                name: "IX_ResourceAllocations_ClientId",
                table: "ResourceAllocations");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "ResourceAllocations");
        }
    }
}
