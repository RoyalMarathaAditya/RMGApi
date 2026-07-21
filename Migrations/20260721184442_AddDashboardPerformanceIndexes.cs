using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMS.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddDashboardPerformanceIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_RA_EmpId_Status_Deleted')
CREATE NONCLUSTERED INDEX [IX_RA_EmpId_Status_Deleted]
    ON [ResourceAllocations] ([EmployeeId], [AllocationStatus], [IsDeleted])
    INCLUDE ([AllocationPercentage], [ProjectId]);
");

            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_RA_Status_Deleted_Includes')
CREATE NONCLUSTERED INDEX [IX_RA_Status_Deleted_Includes]
    ON [ResourceAllocations] ([AllocationStatus], [IsDeleted])
    INCLUDE ([EmployeeId], [AllocationPercentage]);
");

            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Emp_FullName_Code_Includes')
CREATE NONCLUSTERED INDEX [IX_Emp_FullName_Code_Includes]
    ON [Employees] ([FullName], [EmployeeCode])
    INCLUDE ([Id], [DOJ], [PriorExperience], [PracticeId], [SubPracticeId], [DesignationId], [DepartmentTypeId], [StatusId]);
");

            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_EL_FromDate_ToDate')
CREATE NONCLUSTERED INDEX [IX_EL_FromDate_ToDate]
    ON [EmployeeLeaves] ([FromDate], [ToDate])
    INCLUDE ([EmployeeId]);
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP INDEX IF EXISTS [IX_RA_EmpId_Status_Deleted] ON [ResourceAllocations];");
            migrationBuilder.Sql("DROP INDEX IF EXISTS [IX_RA_Status_Deleted_Includes] ON [ResourceAllocations];");
            migrationBuilder.Sql("DROP INDEX IF EXISTS [IX_Emp_FullName_Code_Includes] ON [Employees];");
            migrationBuilder.Sql("DROP INDEX IF EXISTS [IX_EL_FromDate_ToDate] ON [EmployeeLeaves];");
        }
    }
}
