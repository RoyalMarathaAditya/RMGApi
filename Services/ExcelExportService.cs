using HRMS.Api.Data;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace HRMS.Api.Services
{
    public class ExcelExportService : IExcelExportService
    {
        private readonly AppDbContext _dbContext;

        public ExcelExportService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<byte[]> ExportEmployeesAsync(string? fullName = null, Guid? practiceId = null, DateTime? doj = null, Guid? statusId = null, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.Employees
                .AsNoTracking()
                .Include(e => e.EmploymentType)
                .Include(e => e.Location)
                .Include(e => e.Practice)
                .Include(e => e.SubPractice)
                .Include(e => e.EmployeeStatus)
                .Include(e => e.ReportingManager)
                .Include(e => e.PracticeHead)
                .Include(e => e.Designation)
                .Where(e => !e.IsDeleted);

            if (!string.IsNullOrWhiteSpace(fullName))
                query = query.Where(x => x.FullName.Contains(fullName));

            if (practiceId.HasValue)
                query = query.Where(x => x.PracticeId == practiceId.Value);

            if (doj.HasValue)
                query = query.Where(x => x.DOJ.Year == doj.Value.Year && x.DOJ.Month == doj.Value.Month);

            if (statusId.HasValue)
                query = query.Where(x => x.StatusId == statusId.Value);

            var employees = await query.OrderBy(e => e.FullName).ToListAsync(cancellationToken);

            using var package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Employees");

            var headers = new[]
            {
                "Emp Id", "Full Name", "FTE/ Consultant", "Role/Designation",
                "OU 4 - Practice", "OU 5 - Sub-practice", "Location",
                "L1 Manager", "Practice Head", "Email ID",
                "Active", "DOJ", "LWD"
            };

            for (var i = 0; i < headers.Length; i++)
            {
                var cell = sheet.Cells[1, i + 1];
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }

            for (var row = 0; row < employees.Count; row++)
            {
                var e = employees[row];
                var excelRow = row + 2;

                sheet.Cells[excelRow, 1].Value = e.EmployeeCode;
                sheet.Cells[excelRow, 2].Value = e.FullName;
                sheet.Cells[excelRow, 3].Value = e.EmploymentType?.Name;
                sheet.Cells[excelRow, 4].Value = e.Designation?.Name;
                sheet.Cells[excelRow, 5].Value = e.Practice?.Name;
                sheet.Cells[excelRow, 6].Value = e.SubPractice?.Name;
                sheet.Cells[excelRow, 7].Value = e.Location?.Name;
                sheet.Cells[excelRow, 8].Value = e.ReportingManager?.FullName ?? e.ReportingManagerName;
                sheet.Cells[excelRow, 9].Value = e.PracticeHead?.FullName ?? e.PracticeHeadName;
                sheet.Cells[excelRow, 10].Value = e.Email;
                sheet.Cells[excelRow, 11].Value = e.EmployeeStatus?.Name;
                sheet.Cells[excelRow, 12].Value = e.DOJ.ToString("dd-MMM-yyyy", null);
                sheet.Cells[excelRow, 13].Value = e.LWD?.ToString("dd-MMM-yyyy", null);

                for (var col = 1; col <= headers.Length; col++)
                {
                    sheet.Cells[excelRow, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }
            }

            sheet.Cells[1, 1, employees.Count + 1, headers.Length].AutoFitColumns();

            return await package.GetAsByteArrayAsync(cancellationToken);
        }
    }
}