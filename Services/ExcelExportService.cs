using HRMS.Api.Data;
using HRMS.Api.Models.RMG;
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
            ExcelPackage.License.SetNonCommercialOrganization("RMG HRMS");
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

        public async Task<byte[]> ExportDetailedResourceAllocationsAsync(string? searchTerm = null, string? practice = null, string? resourceStatus = null, CancellationToken cancellationToken = default)
        {
            var employees = _dbContext.Employees
                .AsNoTracking()
                .Include(e => e.Designation)
                .Include(e => e.Practice).ThenInclude(p => p.PracticeHead)
                .Include(e => e.SubPractice)
                .Include(e => e.Location)
                .Include(e => e.EmploymentType)
                .Include(e => e.EmployeeStatus)
                .Include(e => e.ReportingManager)
                .Include(e => e.EmployeeSkills).ThenInclude(es => es.Skill)
                .Where(e => !e.IsDeleted);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.ToLower();
                employees = employees.Where(e => e.FullName.ToLower().Contains(term) || e.EmployeeCode.ToLower().Contains(term));
            }

            if (!string.IsNullOrWhiteSpace(practice))
                employees = employees.Where(e => e.Practice != null && e.Practice.Name == practice);

            var employeeList = await employees.OrderBy(e => e.FullName).ToListAsync(cancellationToken);

            using var package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("ResourceAllocation");

            var headers = new[]
            {
                // Employee info
                "Employee Name", "Employee Code", "Designation", "Practice", "Practice Head",
                "Location", "FTE/ Consultant", "L1 Manager", "Email",
                "Primary Skill", "Key Skills", "Sub Practice",
                "Total Experience (yrs)", "NV Experience (yrs)", "Prior Experience (yrs)",
                "Experience Range", "Joining Date", "Billing Status",
                "Active",
                // Project allocation details
                "Project Name", "Project Code", "Client Name", "Project Manager",
                "Delivery Head", "CSM", "Project Status", "Allocation %",
                "Start Date", "End Date", "Billable Status", "Allocation Status",
                "Status", "Engineering", "Billing Bucket", "Aging Bucket",
                "Duration", "Ageing", "Current Billing Status", "Billable Date Probability",
                "Probable Next Assignment", "Probable Next Assignment Date",
                "Action Item", "Remarks"
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

            var excelRow = 2;
            foreach (var emp in employeeList)
            {
                var allocations = await _dbContext.ResourceAllocations
                    .AsNoTracking()
                    .Include(ra => ra.Project).ThenInclude(p => p.Client)
                    .Include(ra => ra.ProjectStatus)
                    .Include(ra => ra.Status)
                    .Include(ra => ra.CurrentBillingStatus)
                    .Include(ra => ra.BillableDateProbability)
                    .Include(ra => ra.BillingBucket)
                    .Include(ra => ra.AgeingBucket)
                    .Include(ra => ra.ProbableNextAssignment)
                    .Where(ra => ra.EmployeeId == emp.Id && !ra.IsDeleted && ra.AllocationStatus != "Cancelled" && ra.AllocationStatus != "Released" && ra.AllocationStatus != "History")
                    .ToListAsync(cancellationToken);

                var totalAllocated = allocations.Sum(a => a.AllocationPercentage);

                if (resourceStatus == "Available" && totalAllocated > 0) continue;
                if (resourceStatus == "Partially Allocated" && (totalAllocated <= 0 || totalAllocated >= 100)) continue;
                if (resourceStatus == "Fully Allocated" && totalAllocated != 100) continue;
                if (resourceStatus == "Overallocated" && totalAllocated <= 100) continue;

                Action<ResourceAllocation?> writeEmployeeRow = alloc =>
                {
                    var startDate = alloc?.StartDate ?? DateTime.MinValue;
                    var endDate = alloc?.EndDate ?? DateTime.UtcNow.Date;
                    var durationDays = alloc != null ? (endDate - startDate).Days : 0;
                    var today = DateTime.UtcNow.Date;
                    var ageingDays = alloc != null
                        ? Math.Max(0, (int)Math.Round(((today - startDate).Days + 1) * (double)alloc.AllocationPercentage / 100.0))
                        : 0;

                    var col = 1;
                    sheet.Cells[excelRow, col++].Value = emp.FullName;
                    sheet.Cells[excelRow, col++].Value = emp.EmployeeCode;
                    sheet.Cells[excelRow, col++].Value = emp.Designation?.Name;
                    sheet.Cells[excelRow, col++].Value = emp.Practice?.Name;
                    sheet.Cells[excelRow, col++].Value = emp.Practice?.PracticeHead?.FullName;
                    sheet.Cells[excelRow, col++].Value = emp.Location?.Name;
                    sheet.Cells[excelRow, col++].Value = emp.EmploymentType?.Name;
                    sheet.Cells[excelRow, col++].Value = emp.ReportingManager?.FullName ?? emp.ReportingManagerName;
                    sheet.Cells[excelRow, col++].Value = emp.Email;

                    var primarySkill = !string.IsNullOrEmpty(emp.PrimarySkillName) ? emp.PrimarySkillName :
                        emp.EmployeeSkills?.Where(es => es.Skill != null).Select(es => es.Skill!.Name).FirstOrDefault();
                    var skillString = !string.IsNullOrEmpty(emp.SkillNames) ? emp.SkillNames :
                        (emp.EmployeeSkills?.Any(es => es.Skill != null) == true
                            ? string.Join(", ", emp.EmployeeSkills.Where(es => es.Skill != null).Select(es => es.Skill!.Name))
                            : null);
                    var totalExperience = Math.Round((emp.PriorExperience ?? 0) + (emp.RelevantExperience ?? 0), 1);
                    var nvExperience = Math.Round(Math.Max(0, totalExperience - (emp.PriorExperience ?? 0)), 1);
                    var experienceRange = GetExperienceRange(totalExperience);
                    var isBillable = allocations.Any(a => a.BillableStatus == "Billable");

                    sheet.Cells[excelRow, col++].Value = primarySkill;
                    sheet.Cells[excelRow, col++].Value = skillString;
                    sheet.Cells[excelRow, col++].Value = emp.SubPractice?.Name;
                    sheet.Cells[excelRow, col++].Value = totalExperience;
                    sheet.Cells[excelRow, col++].Value = nvExperience;
                    sheet.Cells[excelRow, col++].Value = emp.PriorExperience;
                    sheet.Cells[excelRow, col++].Value = experienceRange;
                    sheet.Cells[excelRow, col++].Value = emp.DOJ.ToString("dd-MMM-yyyy");
                    sheet.Cells[excelRow, col++].Value = isBillable ? "Yes" : "No";
                    sheet.Cells[excelRow, col++].Value = emp.EmployeeStatus?.Name;

                    sheet.Cells[excelRow, col++].Value = alloc?.Project?.ProjectName;
                    sheet.Cells[excelRow, col++].Value = alloc?.Project?.ProjectCode;
                    sheet.Cells[excelRow, col++].Value = alloc?.Project?.Client?.Name;
                    sheet.Cells[excelRow, col++].Value = alloc?.Project?.ProjectManager;
                    sheet.Cells[excelRow, col++].Value = alloc?.Project?.DeliveryHead;
                    sheet.Cells[excelRow, col++].Value = alloc?.Project?.CSM;
                    sheet.Cells[excelRow, col++].Value = alloc?.ProjectStatus?.Name ?? alloc?.BillableStatus;
                    sheet.Cells[excelRow, col++].Value = alloc?.AllocationPercentage;
                    sheet.Cells[excelRow, col++].Value = alloc?.StartDate.ToString("dd-MMM-yyyy");
                    sheet.Cells[excelRow, col++].Value = alloc?.EndDate?.ToString("dd-MMM-yyyy");
                    sheet.Cells[excelRow, col++].Value = alloc?.BillableStatus;
                    sheet.Cells[excelRow, col++].Value = alloc?.AllocationStatus;
                    sheet.Cells[excelRow, col++].Value = alloc?.Status?.Name;
                    sheet.Cells[excelRow, col++].Value = alloc?.Engineering ?? (emp.Engineering.HasValue ? (emp.Engineering.Value ? "Yes" : "No") : null);
                    sheet.Cells[excelRow, col++].Value = alloc?.BillingBucket?.Name;
                    sheet.Cells[excelRow, col++].Value = alloc?.AgeingBucket?.Name;
                    sheet.Cells[excelRow, col++].Value = alloc != null ? $"{(alloc.Duration ?? durationDays)} Days" : null;
                    sheet.Cells[excelRow, col++].Value = alloc != null ? $"{Math.Max(0, alloc.Ageing ?? ageingDays)} Days" : null;
                    sheet.Cells[excelRow, col++].Value = alloc?.CurrentBillingStatus?.Name;
                    sheet.Cells[excelRow, col++].Value = alloc?.BillableDateProbability?.Name;
                    sheet.Cells[excelRow, col++].Value = alloc?.ProbableNextAssignment?.Name;
                    sheet.Cells[excelRow, col++].Value = alloc?.ProbableNextAssignmentDate?.ToString("dd-MMM-yyyy");
                    sheet.Cells[excelRow, col++].Value = alloc?.ActionItem;
                    sheet.Cells[excelRow, col++].Value = alloc?.Remarks;

                    for (var c = 1; c <= headers.Length; c++)
                    {
                        sheet.Cells[excelRow, c].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    }

                    excelRow++;
                };

                if (allocations.Count > 0)
                {
                    foreach (var a in allocations)
                    {
                        writeEmployeeRow(a);
                    }
                }
                else
                {
                    writeEmployeeRow(null);
                }
            }

            sheet.Cells[1, 1, excelRow - 1, headers.Length].AutoFitColumns();

            return await package.GetAsByteArrayAsync(cancellationToken);
        }

        private static string GetExperienceRange(decimal totalExperience)
        {
            if (totalExperience <= 2) return "0 - 2 Years";
            if (totalExperience <= 6) return "3 - 6 Years";
            if (totalExperience <= 9) return "6 - 9 Years";
            if (totalExperience <= 12) return "9 - 12 Years";
            return "More than 12 Years";
        }
    }
}