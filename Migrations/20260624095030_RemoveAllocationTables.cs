using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HRMS.Api.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAllocationTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectAllocations");

            migrationBuilder.DropTable(
                name: "AllocationStatusMasters");

            migrationBuilder.DropTable(
                name: "EmployeeProjectStatusMasters");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AllocationStatusMasters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllocationStatusMasters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeProjectStatusMasters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeProjectStatusMasters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectAllocations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AllocationStatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    EmployeeProjectStatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    AllocationEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AllocationPercentage = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    AllocationStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BillablePercentage = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsBillable = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsUtilized = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextAssignmentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProbableBillableDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectAllocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectAllocations_AllocationStatusMasters_AllocationStatusId",
                        column: x => x.AllocationStatusId,
                        principalTable: "AllocationStatusMasters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectAllocations_EmployeeProjectStatusMasters_EmployeeProjectStatusId",
                        column: x => x.EmployeeProjectStatusId,
                        principalTable: "EmployeeProjectStatusMasters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectAllocations_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectAllocations_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "AllocationStatusMasters",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "IsActive", "IsDeleted", "ModifiedBy", "ModifiedOn", "Name" },
                values: new object[,]
                {
                    { new Guid("90000000-0000-0000-0000-000000000001"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, null, null, "Current" },
                    { new Guid("90000000-0000-0000-0000-000000000002"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, null, null, "History" }
                });

            migrationBuilder.InsertData(
                table: "EmployeeProjectStatusMasters",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "IsActive", "IsDeleted", "ModifiedBy", "ModifiedOn", "Name" },
                values: new object[,]
                {
                    { new Guid("a0000000-0000-0000-0000-000000000001"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, null, null, "Billable" },
                    { new Guid("a0000000-0000-0000-0000-000000000002"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, null, null, "Scoped" },
                    { new Guid("a0000000-0000-0000-0000-000000000003"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, null, null, "To Be Scoped" },
                    { new Guid("a0000000-0000-0000-0000-000000000004"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, null, null, "Utilized" },
                    { new Guid("a0000000-0000-0000-0000-000000000005"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, null, null, "Onboarding" },
                    { new Guid("a0000000-0000-0000-0000-000000000006"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, null, null, "Training" },
                    { new Guid("a0000000-0000-0000-0000-000000000007"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, null, null, "PIP" },
                    { new Guid("a0000000-0000-0000-0000-000000000008"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, null, null, "Management" },
                    { new Guid("a0000000-0000-0000-0000-000000000009"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, null, null, "Long Leave" },
                    { new Guid("a0000000-0000-0000-0000-000000000010"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, null, null, "1$ Utilized" },
                    { new Guid("a0000000-0000-0000-0000-000000000011"), null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, null, null, "Shadowing" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AllocationStatusMasters_Name",
                table: "AllocationStatusMasters",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeProjectStatusMasters_Name",
                table: "EmployeeProjectStatusMasters",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAllocations_AllocationStatusId",
                table: "ProjectAllocations",
                column: "AllocationStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAllocations_EmployeeId",
                table: "ProjectAllocations",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAllocations_EmployeeId_ProjectId_AllocationStatusId",
                table: "ProjectAllocations",
                columns: new[] { "EmployeeId", "ProjectId", "AllocationStatusId" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAllocations_EmployeeProjectStatusId",
                table: "ProjectAllocations",
                column: "EmployeeProjectStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAllocations_ProjectId",
                table: "ProjectAllocations",
                column: "ProjectId");
        }
    }
}
