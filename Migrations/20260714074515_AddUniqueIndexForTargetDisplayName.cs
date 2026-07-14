using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMS.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexForTargetDisplayName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ColumnMappings_EntityType_TargetDisplayName",
                table: "ColumnMappings",
                columns: new[] { "EntityType", "TargetDisplayName" },
                unique: true,
                filter: "[IsActive] = 1 AND [TargetDisplayName] IS NOT NULL AND [TargetDisplayName] != ''");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ColumnMappings_EntityType_TargetDisplayName",
                table: "ColumnMappings");
        }
    }
}
