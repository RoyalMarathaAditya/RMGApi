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

        public async Task<IEnumerable<PracticeWiseReportDto>> GetPracticeWiseReportAsync(bool engineeringOnly = false, CancellationToken cancellationToken = default)
        {
            var sql = engineeringOnly
                ? "EXEC usp_GetPracticeWiseReport @EngineeringOnly = 1"
                : "EXEC usp_GetPracticeWiseReport @EngineeringOnly = 0";
            var rows = await _dbContext.Database
                .SqlQueryRaw<PracticeWiseReportSpResult>(sql)
                .ToListAsync(cancellationToken);

            return rows.Select(r => new PracticeWiseReportDto
            {
                PracticeId = r.PracticeId,
                PracticeName = r.PracticeName,
                TotalHeadcount = r.TotalHeadcount,
                BillableCount = r.BillableCount,
                UtilizedCount = r.UtilizedCount,
                EngineeringHeadcount = r.EngineeringHeadcount,
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

        public async Task<byte[]> ExportPracticeWiseReportAsync(bool engineeringOnly = false, CancellationToken cancellationToken = default)
        {
            var data = await GetPracticeWiseReportAsync(engineeringOnly, cancellationToken);
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

        // ───────── Client Wise Report ─────────

        public async Task<IEnumerable<ClientWiseReportDto>> GetClientWiseReportAsync(string? clientIds, string? statusFilter, CancellationToken cancellationToken = default)
        {
            var clientsQuery = from c in _dbContext.Clients.Where(c => !c.IsDeleted)
                               select new
                               {
                                   c.Id,
                                   c.Name,
                                   TotalProjects = c.Projects.Count(p => !p.IsDeleted),
                                   ActiveProjects = c.Projects.Count(p => !p.IsDeleted && p.IsActive),
                                   CompletedProjects = c.Projects.Count(p => !p.IsDeleted && !p.IsActive),
                               };

            if (!string.IsNullOrEmpty(clientIds))
            {
                var ids = clientIds.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Select(id => int.TryParse(id, out var parsed) ? parsed : (int?)null)
                    .Where(id => id.HasValue)
                    .Select(id => id!.Value)
                    .ToHashSet();
                if (ids.Count > 0)
                    clientsQuery = clientsQuery.Where(c => ids.Contains(c.Id));
            }

            if (statusFilter == "Active")
                clientsQuery = clientsQuery.Where(c => c.ActiveProjects > 0);
            else if (statusFilter == "Completed")
                clientsQuery = clientsQuery.Where(c => c.CompletedProjects > 0);

            var clients = await clientsQuery.OrderBy(c => c.Name).ToListAsync(cancellationToken);
            var selectedClientIds = clients.Select(c => c.Id).ToHashSet();

            var allocData = await (from ra in _dbContext.ResourceAllocations.Where(r => !r.IsDeleted)
                                    join e in _dbContext.Employees.Where(emp => !emp.IsDeleted) on ra.EmployeeId equals e.Id
                                    join pr in _dbContext.Projects.Where(p => !p.IsDeleted) on ra.ProjectId equals pr.Id
                                    where selectedClientIds.Contains(pr.ClientId)
                                    select new { ra, e, pr.ClientId })
                                    .ToListAsync(cancellationToken);

            var allocGrouped = allocData.GroupBy(x => x.ClientId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var projectData = await (from ra in _dbContext.ResourceAllocations.Where(r => !r.IsDeleted)
                                      join pr in _dbContext.Projects.Where(p => !p.IsDeleted) on ra.ProjectId equals pr.Id
                                      where selectedClientIds.Contains(pr.ClientId)
                                      select new { ra, pr })
                                     .GroupBy(x => x.pr.Id)
                                     .Select(g => new
                                     {
                                         g.First().pr.Id,
                                         g.First().pr.ProjectName,
                                         g.First().pr.IsActive,
                                         g.First().pr.ClientId,
                                         EmployeeCount = g.Select(x => x.ra.EmployeeId).Distinct().Count(),
                                         AvgAllocation = g.Average(x => (decimal?)x.ra.AllocationPercentage) ?? 0m,
                                     })
                                     .ToListAsync(cancellationToken);

            var result = clients.Select(c =>
            {
                allocGrouped.TryGetValue(c.Id, out var allocs);
                var totalEmps = allocs?.Select(x => x.e.Id).Distinct().Count() ?? 0;
                var billableCount = allocs?.Where(x => x.ra.BillableStatus == "Billable").Select(x => x.e.Id).Distinct().Count() ?? 0;
                var avgAlloc = allocs?.Average(x => (decimal?)x.ra.AllocationPercentage) ?? 0m;

                return new ClientWiseReportDto
                {
                    ClientId = c.Id,
                    ClientName = c.Name,
                    TotalProjects = c.TotalProjects,
                    ActiveProjects = c.ActiveProjects,
                    CompletedProjects = c.CompletedProjects,
                    TotalEmployees = totalEmps,
                    BillableCount = billableCount,
                    NonBillableCount = totalEmps - billableCount,
                    AvgAllocation = avgAlloc,
                    Projects = projectData
                        .Where(pd => pd.ClientId == c.Id)
                        .Select(pd => new ClientProjectDto
                        {
                            ProjectId = pd.Id,
                            ProjectName = pd.ProjectName,
                            Status = pd.IsActive ? "Active" : "Completed",
                            EmployeeCount = pd.EmployeeCount,
                            AvgAllocation = pd.AvgAllocation,
                        })
                        .ToList(),
                };
            }).ToList();

            return result;
        }

        public async Task<ReportChartDataDto> GetClientWiseChartDataAsync(CancellationToken cancellationToken = default)
        {
            var clients = await _dbContext.Clients.Where(c => !c.IsDeleted).ToListAsync(cancellationToken);
            var clientIds = clients.Select(c => c.Id).ToHashSet();

            var projects = await _dbContext.Projects.Where(p => !p.IsDeleted && clientIds.Contains(p.ClientId)).ToListAsync(cancellationToken);

            var allocData = await (from ra in _dbContext.ResourceAllocations.Where(r => !r.IsDeleted)
                                    join e in _dbContext.Employees.Where(emp => !emp.IsDeleted) on ra.EmployeeId equals e.Id
                                    join pr in _dbContext.Projects.Where(p => !p.IsDeleted) on ra.ProjectId equals pr.Id
                                    where clientIds.Contains(pr.ClientId)
                                    select new { ra, e, pr })
                                    .ToListAsync(cancellationToken);

            var employeeBillable = allocData
                .GroupBy(x => x.e.Id)
                .Select(g => new { EmployeeId = g.Key, IsBillable = g.Any(x => x.ra.BillableStatus == "Billable") })
                .ToList();

            var totalBillable = employeeBillable.Count(x => x.IsBillable);
            var totalNonBillable = employeeBillable.Count(x => !x.IsBillable);

            var utilizationByEntity = allocData
                .GroupBy(x => x.pr.ClientId)
                .Select(g => new BarDataDto
                {
                    Name = clients.FirstOrDefault(c => c.Id == g.Key)?.Name ?? "Unknown",
                    Utilization = g.Average(x => (decimal?)x.ra.AllocationPercentage) ?? 0m,
                })
                .OrderByDescending(x => x.Utilization)
                .Take(10)
                .ToList();

            var projectsByStatus = projects
                .GroupBy(p => p.IsActive ? "Active" : "Completed")
                .Select(g => new PieDataDto { Name = g.Key, Count = g.Count() })
                .ToList();

            var monthlyTrend = allocData
                .Where(x => x.ra.StartDate.Year >= DateTime.UtcNow.AddYears(-1).Year)
                .GroupBy(x => new { x.ra.StartDate.Year, x.ra.StartDate.Month })
                .Select(g => new LineDataDto
                {
                    Month = $"{g.Key.Year}-{g.Key.Month:D2}",
                    Started = g.Select(x => x.ra.EmployeeId).Distinct().Count(),
                })
                .OrderBy(x => x.Month)
                .ToList();

            return new ReportChartDataDto
            {
                BillableDistribution = new List<DonutDataDto>
                {
                    new() { Name = "Billable", Value = totalBillable },
                    new() { Name = "Non-Billable", Value = totalNonBillable },
                },
                UtilizationByEntity = utilizationByEntity,
                ProjectsByStatus = projectsByStatus,
                MonthlyTrend = monthlyTrend,
            };
        }

        // ───────── Project Wise Report ─────────

        public async Task<IEnumerable<ProjectWiseReportDto>> GetProjectWiseReportAsync(string? clientFilter, string? practiceFilter, string? statusFilter, string? searchFilter, CancellationToken cancellationToken = default)
        {
            var projectQuery = from pr in _dbContext.Projects.Where(p => !p.IsDeleted)
                               select new
                               {
                                   pr.Id,
                                   pr.ProjectName,
                                   pr.ProjectCode,
                                   pr.ClientId,
                                   ClientName = pr.Client.Name,
                                   pr.ProjectManager,
                                   pr.ProjectStartDate,
                                   pr.ProjectEndDate,
                                   pr.IsActive,
                               };

            if (!string.IsNullOrEmpty(clientFilter) && int.TryParse(clientFilter, out var clientId))
                projectQuery = projectQuery.Where(p => p.ClientId == clientId);

            if (statusFilter == "Active")
                projectQuery = projectQuery.Where(p => p.IsActive);
            else if (statusFilter == "Completed")
                projectQuery = projectQuery.Where(p => !p.IsActive);

            if (!string.IsNullOrEmpty(searchFilter))
                projectQuery = projectQuery.Where(p => p.ProjectName.Contains(searchFilter));

            var projects = await projectQuery.OrderBy(p => p.ProjectName).ToListAsync(cancellationToken);
            var projectIds = projects.Select(p => p.Id).ToHashSet();

            var allocData = await (from ra in _dbContext.ResourceAllocations.Where(r => !r.IsDeleted)
                                   join e in _dbContext.Employees.Where(emp => !emp.IsDeleted) on ra.EmployeeId equals e.Id
                                   where projectIds.Contains(ra.ProjectId)
                                   select new { ra, e })
                                   .ToListAsync(cancellationToken);

            if (!string.IsNullOrEmpty(practiceFilter) && Guid.TryParse(practiceFilter, out var practiceGuid))
                allocData = allocData.Where(x => x.e.PracticeId == practiceGuid).ToList();

            var allocGrouped = allocData.GroupBy(x => x.ra.ProjectId);
            var allocLookup = allocGrouped.ToDictionary(g => g.Key, g => g.ToList());

            var result = projects.Select(pr =>
            {
                allocLookup.TryGetValue(pr.Id, out var allocs);
                var totalEmps = allocs?.Select(x => x.e.Id).Distinct().Count() ?? 0;
                var billableCount = allocs?.Where(x => x.ra.BillableStatus == "Billable").Select(x => x.e.Id).Distinct().Count() ?? 0;
                var avgAlloc = allocs?.Average(x => (decimal?)x.ra.AllocationPercentage) ?? 0m;

                return new ProjectWiseReportDto
                {
                    ProjectId = pr.Id,
                    ProjectName = pr.ProjectName,
                    ProjectCode = pr.ProjectCode,
                    ClientName = pr.ClientName,
                    ProjectManager = pr.ProjectManager,
                    ProjectStartDate = pr.ProjectStartDate,
                    ProjectEndDate = pr.ProjectEndDate,
                    TotalEmployees = totalEmps,
                    BillableCount = billableCount,
                    AvgAllocation = avgAlloc,
                    Status = pr.IsActive ? "Active" : "Completed",
                    Employees = (allocs ?? new())
                        .GroupBy(x => x.e.Id)
                        .Select(g => g.First())
                        .Select(x => new ProjectEmployeeDto
                        {
                            EmployeeCode = x.e.EmployeeCode,
                            FullName = x.e.FullName,
                            AllocationPercentage = x.ra.AllocationPercentage,
                            BillableStatus = x.ra.BillableStatus,
                        })
                        .ToList(),
                };
            }).ToList();

            return result;
        }

        public async Task<ReportChartDataDto> GetProjectWiseChartDataAsync(CancellationToken cancellationToken = default)
        {
            var allocData = await (from ra in _dbContext.ResourceAllocations.Where(r => !r.IsDeleted)
                                    join e in _dbContext.Employees.Where(emp => !emp.IsDeleted) on ra.EmployeeId equals e.Id
                                    join pr in _dbContext.Projects.Where(p => !p.IsDeleted) on ra.ProjectId equals pr.Id
                                    select new { ra, e, pr })
                                    .ToListAsync(cancellationToken);

            var employeeBillable = allocData
                .GroupBy(x => x.e.Id)
                .Select(g => new { EmployeeId = g.Key, IsBillable = g.Any(x => x.ra.BillableStatus == "Billable") })
                .ToList();

            var totalBillable = employeeBillable.Count(x => x.IsBillable);
            var totalNonBillable = employeeBillable.Count(x => !x.IsBillable);

            var utilizationByEntity = allocData
                .GroupBy(x => x.pr.ProjectName)
                .Select(g => new BarDataDto
                {
                    Name = g.Key,
                    Utilization = g.Average(x => (decimal?)x.ra.AllocationPercentage) ?? 0m,
                })
                .OrderByDescending(x => x.Utilization)
                .Take(10)
                .ToList();

            var projectsByStatus = allocData
                .GroupBy(x => x.pr.IsActive ? "Active" : "Completed")
                .Select(g => new PieDataDto { Name = g.Key, Count = g.Select(x => x.pr.Id).Distinct().Count() })
                .ToList();

            var now = DateTime.UtcNow;
            var monthlyTrend = allocData
                .Where(x => x.ra.StartDate.Year >= now.AddYears(-1).Year)
                .GroupBy(x => new { x.ra.StartDate.Year, x.ra.StartDate.Month })
                .Select(g => new LineDataDto
                {
                    Month = $"{g.Key.Year}-{g.Key.Month:D2}",
                    Started = g.Select(x => x.ra.EmployeeId).Distinct().Count(),
                })
                .OrderBy(x => x.Month)
                .ToList();

            return new ReportChartDataDto
            {
                BillableDistribution = new List<DonutDataDto>
                {
                    new() { Name = "Billable", Value = totalBillable },
                    new() { Name = "Non-Billable", Value = totalNonBillable },
                },
                UtilizationByEntity = utilizationByEntity,
                ProjectsByStatus = projectsByStatus,
                MonthlyTrend = monthlyTrend,
            };
        }
    }
}
