using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMS.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddPracticeWiseReportSp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE OR ALTER PROCEDURE usp_GetPracticeWiseReport
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        p.Id AS PracticeId,
        p.Name AS PracticeName,
        COUNT(DISTINCT e.Id) AS TotalHeadcount,
        COUNT(DISTINCT CASE WHEN ra.Id IS NOT NULL AND ra.BillableStatus = 'Billable' 
                        THEN e.Id END) AS BillableCount,
        COUNT(DISTINCT CASE WHEN ra.Id IS NOT NULL THEN e.Id END) AS UtilizedCount,
        COUNT(CASE WHEN exp.TotalExp < 1 THEN 1 END) AS RangeLessThan1,
        COUNT(CASE WHEN exp.TotalExp >= 1 AND exp.TotalExp < 3 THEN 1 END) AS Range1to3,
        COUNT(CASE WHEN exp.TotalExp >= 3 AND exp.TotalExp < 6 THEN 1 END) AS Range3to6,
        COUNT(CASE WHEN exp.TotalExp >= 6 AND exp.TotalExp < 9 THEN 1 END) AS Range6to9,
        COUNT(CASE WHEN exp.TotalExp >= 9 AND exp.TotalExp < 12 THEN 1 END) AS Range9to12,
        COUNT(CASE WHEN exp.TotalExp >= 12 THEN 1 END) AS RangeMoreThan12
    FROM Practices p
    INNER JOIN Employees e ON e.PracticeId = p.Id AND e.IsDeleted = 0
    LEFT JOIN ResourceAllocations ra ON ra.EmployeeId = e.Id 
        AND ra.IsDeleted = 0 AND ra.AllocationStatus IN ('Active', 'Current')
    CROSS APPLY (
        SELECT (DATEDIFF(DAY, e.DOJ, GETUTCDATE()) / 365.25) 
               + ISNULL(e.PriorExperience, 0) AS TotalExp
    ) exp
    WHERE p.IsDeleted = 0 AND p.IsActive = 1
    GROUP BY p.Id, p.Name
    ORDER BY p.Name;
END;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS usp_GetPracticeWiseReport;");
        }
    }
}
