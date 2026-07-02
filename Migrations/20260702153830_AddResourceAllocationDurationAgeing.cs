using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMS.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddResourceAllocationDurationAgeing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Ageing",
                table: "ResourceAllocations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "ResourceAllocations",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ageing",
                table: "ResourceAllocations");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "ResourceAllocations");
        }
    }
}
