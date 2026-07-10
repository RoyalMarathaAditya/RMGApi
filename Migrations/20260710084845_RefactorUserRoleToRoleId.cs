using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMS.Api.Migrations
{
    /// <inheritdoc />
    public partial class RefactorUserRoleToRoleId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Add RoleId as nullable
            migrationBuilder.AddColumn<Guid>(
                name: "RoleId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: true);

            // Step 2: Migrate existing Role text values to RoleId FK
            migrationBuilder.Sql(@"
                UPDATE u
                SET u.RoleId = r.Id
                FROM [dbo].[Users] u
                INNER JOIN [dbo].[RoleMasters] r ON r.[Name] = u.[Role] AND r.[IsDeleted] = 0;

                UPDATE [dbo].[Users]
                SET RoleId = '77777777-7777-7777-7777-777777777777'
                WHERE RoleId IS NULL;
            ");

            // Step 3: Make RoleId non-nullable
            migrationBuilder.AlterColumn<Guid>(
                name: "RoleId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            // Step 4: Drop the old Role column
            migrationBuilder.DropColumn(
                name: "Role",
                table: "Users");

            // Step 5: Add Super Admin role if not exists
            migrationBuilder.InsertData(
                table: "RoleMasters",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "Description", "IsActive", "IsDeleted", "ModifiedBy", "ModifiedOn", "Name" },
                values: new object[] { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, false, null, null, "Super Admin" });

            // Step 6: Create index and FK
            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_RoleMasters_RoleId",
                table: "Users",
                column: "RoleId",
                principalTable: "RoleMasters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_RoleMasters_RoleId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_RoleId",
                table: "Users");

            migrationBuilder.DeleteData(
                table: "RoleMasters",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"));

            // Restore Role column as nullable string
            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            // Restore Role text values from RoleId lookup
            migrationBuilder.Sql(@"
                UPDATE u
                SET u.Role = r.Name
                FROM [dbo].[Users] u
                INNER JOIN [dbo].[RoleMasters] r ON r.[Id] = u.[RoleId];
            ");

            // Make Role non-nullable
            migrationBuilder.Sql("UPDATE [dbo].[Users] SET Role = 'Employee' WHERE Role IS NULL");
            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "Users");
        }
    }
}
