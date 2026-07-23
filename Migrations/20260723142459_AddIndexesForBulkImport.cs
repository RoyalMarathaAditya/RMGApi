using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMS.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexesForBulkImport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Index on BatchId: all SP queries filter by BatchId, without this every query scans the full table
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_EmployeeImportStaging_BatchId')
                    CREATE INDEX IX_EmployeeImportStaging_BatchId ON EmployeeImportStaging(BatchId);
            ");

            // Index on Email: the SP JOINs staging on Email = Employees.Email for UPDATE/INSERT matching
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_EmployeeImportStaging_Email')
                    CREATE INDEX IX_EmployeeImportStaging_Email ON EmployeeImportStaging(Email);
            ");

            // Clean up orphaned staging rows from previous failed imports that accumulate over time
            migrationBuilder.Sql(@"
                DELETE FROM EmployeeImportStaging 
                WHERE BatchId NOT IN (SELECT BatchId FROM EmployeeImportHistory)
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP INDEX IF EXISTS IX_EmployeeImportStaging_BatchId ON EmployeeImportStaging");
            migrationBuilder.Sql("DROP INDEX IF EXISTS IX_EmployeeImportStaging_Email ON EmployeeImportStaging");
        }
    }
}
