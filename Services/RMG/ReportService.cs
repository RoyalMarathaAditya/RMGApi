using HRMS.Api.Data;
using HRMS.Api.DTOs.ReportDtos;
using HRMS.Api.Services.Interfaces.RMG;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace HRMS.Api.Services.RMG
{
    public class ReportService : IReportService
    {
        private readonly AppDbContext _dbContext;

        private static readonly string[] RangeLabels =
        [
            "Less than 1 Year", "1-3 Years", "3-6 Years",
            "6-9 Years", "9-12 Years", "More than 12 Years",
        ];

        public ReportService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<PracticeWiseReportDto>> GetPracticeWiseReportAsync(CancellationToken cancellationToken = default)
        {
            var rows = await _dbContext.Database
                .SqlQueryRaw<PracticeWiseReportSpResult>("EXEC usp_GetPracticeWiseReport")
                .ToListAsync(cancellationToken);

            return rows.Select(r => new PracticeWiseReportDto
            {
                PracticeId = r.PracticeId,
                PracticeName = r.PracticeName,
                TotalHeadcount = r.TotalHeadcount,
                BillableCount = r.BillableCount,
                UtilizedCount = r.UtilizedCount,
                ExperienceRanges =
                [
                    new() { Range = "Less than 1 Year", Count = r.RangeLessThan1 },
                    new() { Range = "1-3 Years", Count = r.Range1to3 },
                    new() { Range = "3-6 Years", Count = r.Range3to6 },
                    new() { Range = "6-9 Years", Count = r.Range6to9 },
                    new() { Range = "9-12 Years", Count = r.Range9to12 },
                    new() { Range = "More than 12 Years", Count = r.RangeMoreThan12 },
                ],
            });
        }

        public async Task<byte[]> ExportPracticeWiseReportAsync(CancellationToken cancellationToken = default)
        {
            var data = await GetPracticeWiseReportAsync(cancellationToken);
            var list = data.ToList();

            ExcelPackage.License.SetNonCommercialOrganization("RMG HRMS");

            using var package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("PracticeWiseReport");

            var headers = new[]
            {
                "Practice", "Total Headcount", "Billable", "Non-Billable",
                "Utilized", "Non-Utilized", "Billability %", "Utilization %",
            }.Concat(RangeLabels).ToArray();

            for (var i = 0; i < headers.Length; i++)
            {
                var cell = sheet.Cells[1, i + 1];
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }

            var rangeLabels = RangeLabels.ToList();

            for (var row = 0; row < list.Count; row++)
            {
                var p = list[row];
                var excelRow = row + 2;
                var expMap = p.ExperienceRanges.ToDictionary(e => e.Range, e => e.Count);

                var col = 1;
                sheet.Cells[excelRow, col++].Value = p.PracticeName;
                sheet.Cells[excelRow, col++].Value = p.TotalHeadcount;
                sheet.Cells[excelRow, col++].Value = p.BillableCount;
                sheet.Cells[excelRow, col++].Value = p.NonBillableCount;
                sheet.Cells[excelRow, col++].Value = p.UtilizedCount;
                sheet.Cells[excelRow, col++].Value = p.NonUtilizedCount;
                sheet.Cells[excelRow, col++].Value = p.BillabilityPercentage;
                sheet.Cells[excelRow, col++].Value = p.UtilizationPercentage;

                foreach (var label in rangeLabels)
                {
                    sheet.Cells[excelRow, col++].Value = expMap.GetValueOrDefault(label, 0);
                }

                for (var c = 1; c <= headers.Length; c++)
                {
                    sheet.Cells[excelRow, c].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }
            }

            sheet.Cells[1, 1, list.Count + 1, headers.Length].AutoFitColumns();

            return await package.GetAsByteArrayAsync(cancellationToken);
        }
    }
}
