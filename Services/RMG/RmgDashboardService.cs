using HRMS.Api.Data;
using HRMS.Api.DTOs.AllocationDtos;
using HRMS.Api.DTOs.RmgDashboardDtos;
using HRMS.Api.Services.Interfaces.RMG;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Services.RMG
{
    public class RmgDashboardService : IRmgDashboardService
    {
        private readonly AppDbContext _dbContext;

        public RmgDashboardService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default)
        {
            var totalEmployees = await _dbContext.Employees.CountAsync(e => !e.IsDeleted, cancellationToken);
            var totalPractices = await _dbContext.Practices.CountAsync(p => !p.IsDeleted && p.IsActive, cancellationToken);

            var activeStatuses = new[] { "Active", "Current" };
            var employeeTotals = await _dbContext.ResourceAllocations
                .Where(ra => !ra.IsDeleted && activeStatuses.Contains(ra.AllocationStatus))
                .GroupBy(ra => ra.EmployeeId)
                .Select(g => new { EmployeeId = g.Key, Total = g.Sum(ra => ra.AllocationPercentage) })
                .ToListAsync(cancellationToken);

            var employeeIdsWithAllocations = employeeTotals.Select(a => a.EmployeeId).ToHashSet();

            var availableCount = employeeTotals.Count(a => a.Total == 0)
                + (totalEmployees - employeeIdsWithAllocations.Count);
            var allocatedCount = employeeTotals.Count(a => a.Total > 0 && a.Total < 100);
            var fullyAllocatedCount = employeeTotals.Count(a => a.Total == 100);
            var overallocatedCount = employeeTotals.Count(a => a.Total > 100);

            var onLeaveCount = await _dbContext.EmployeeLeaves
                .CountAsync(el => el.FromDate <= DateTime.UtcNow && el.ToDate >= DateTime.UtcNow, cancellationToken);

            var practices = await _dbContext.Practices
                .Where(p => !p.IsDeleted && p.IsActive)
                .Select(p => new { p.Id, EmployeeCount = p.Employees.Count(e => !e.IsDeleted) })
                .ToListAsync(cancellationToken);

            var utilizedEmployees = await _dbContext.ResourceAllocations
                .Where(ra => !ra.IsDeleted && activeStatuses.Contains(ra.AllocationStatus))
                .Select(ra => ra.EmployeeId)
                .Distinct()
                .CountAsync(cancellationToken);

            var totalPracticeResources = practices.Sum(p => p.EmployeeCount);
            var practiceUtilization = totalPracticeResources > 0
                ? Math.Round((decimal)utilizedEmployees / totalPracticeResources * 100, 2)
                : 0;

            return new DashboardSummaryDto
            {
                TotalEmployees = totalEmployees,
                TotalPractices = totalPractices,
                AvailableResources = availableCount,
                AllocatedResources = allocatedCount,
                FullyAllocatedResources = fullyAllocatedCount,
                OverallocatedResources = overallocatedCount,
                BenchResources = availableCount,
                ResourcesOnLeave = onLeaveCount,
                PracticeUtilizationPercentage = practiceUtilization
            };
        }

        public async Task<IEnumerable<DashboardGridDto>> GetGridDataAsync(DashboardFilterDto? filter = null, CancellationToken cancellationToken = default)
        {
            var employees = _dbContext.Employees
                .AsNoTracking()
                .Include(e => e.Designation)
                .Include(e => e.DepartmentType)
                .Include(e => e.Practice).ThenInclude(p => p.PracticeHead)
                .Include(e => e.SubPractice)
                .Include(e => e.EmployeeSkills).ThenInclude(es => es.Skill)
                .Include(e => e.EmployeeLeaves)
                .Where(e => !e.IsDeleted);

            if (!string.IsNullOrEmpty(filter?.SearchTerm))
            {
                var term = filter.SearchTerm.ToLower();
                employees = employees.Where(e => e.FullName.ToLower().Contains(term)
                    || e.EmployeeCode.ToLower().Contains(term));
            }

            if (!string.IsNullOrEmpty(filter?.EmployeeName))
            {
                employees = employees.Where(e => e.FullName.Contains(filter.EmployeeName));
            }

            if (!string.IsNullOrEmpty(filter?.EmployeeCode))
            {
                employees = employees.Where(e => e.EmployeeCode.Contains(filter.EmployeeCode));
            }

            if (!string.IsNullOrEmpty(filter?.Practice))
            {
                employees = employees.Where(e => e.Practice.Name == filter.Practice);
            }

            if (!string.IsNullOrEmpty(filter?.Designation))
            {
                employees = employees.Where(e => e.Designation != null && e.Designation.Name == filter.Designation);
            }

            var employeeList = await employees.ToListAsync(cancellationToken);
            var result = new List<DashboardGridDto>();

            foreach (var emp in employeeList)
            {
                var activeAllocations = await _dbContext.ResourceAllocations
                    .Where(ra => ra.EmployeeId == emp.Id && !ra.IsDeleted && (ra.AllocationStatus == "Active" || ra.AllocationStatus == "Current"))
                    .ToListAsync(cancellationToken);

                var projectNames = activeAllocations.Select(ra => ra.Project?.ProjectName ?? "").Where(n => !string.IsNullOrEmpty(n)).ToList();
                var totalAllocated = activeAllocations.Sum(ra => ra.AllocationPercentage);
                var currentProject = projectNames.Count > 1 ? $"{projectNames[0]} (+{projectNames.Count - 1})" : projectNames.FirstOrDefault();
                var allocationStatus = activeAllocations.FirstOrDefault()?.AllocationStatus;

                var onLeave = emp.EmployeeLeaves?.Any(el => el.FromDate <= DateTime.UtcNow && el.ToDate >= DateTime.UtcNow) ?? false;

                var resourceStatus = onLeave ? "On Leave"
                    : totalAllocated > 100 ? "Overallocated"
                    : totalAllocated >= 100 ? "Fully Allocated"
                    : totalAllocated > 0 ? "Partially Allocated"
                    : "Available";

                if (!string.IsNullOrEmpty(filter?.ResourceStatus) && resourceStatus != filter.ResourceStatus)
                    continue;

                if (!string.IsNullOrEmpty(filter?.AllocationStatus) && allocationStatus != filter.AllocationStatus)
                    continue;

                var totalExperience = Math.Round(
                    ((decimal)(DateTime.UtcNow - emp.DOJ).TotalDays / 365.25m) + (emp.PriorExperience ?? 0),
                    1);

                result.Add(new DashboardGridDto
                {
                    EmployeeId = emp.Id,
                    EmployeeName = emp.FullName,
                    EmployeeCode = emp.EmployeeCode,
                    Designation = emp.Designation?.Name,
                    Department = emp.DepartmentType?.Name,
                    Practice = emp.Practice?.Name ?? "",
                    PracticeHead = emp.Practice?.PracticeHead?.FullName,
                    SubPractice = emp.SubPractice?.Name,
                    TotalExperience = totalExperience,
                    Skills = emp.EmployeeSkills != null ? string.Join(", ", emp.EmployeeSkills.Where(es => es.Skill != null).Select(es => es.Skill!.Name)) : null,
                    CurrentProject = currentProject,
                    Projects = projectNames,
                    AllocationPercentage = totalAllocated,
                    AvailableCapacity = Math.Max(0, 100 - totalAllocated),
                    ResourceStatus = resourceStatus,
                    AllocationStatus = allocationStatus
                });
            }

            return result.OrderBy(r => r.EmployeeName);
        }

        public async Task<IEnumerable<ResourceSuggestionDto>> GetSuitableResourcesAsync(int projectId, CancellationToken cancellationToken = default)
        {
            var project = await _dbContext.Projects
                .FirstOrDefaultAsync(p => p.Id == projectId, cancellationToken);

            if (project is null) return Enumerable.Empty<ResourceSuggestionDto>();

            var employees = await _dbContext.Employees
                .AsNoTracking()
                .Include(e => e.EmployeeSkills).ThenInclude(es => es.Skill)
                .Include(e => e.Practice)
                .Where(e => !e.IsDeleted)
                .ToListAsync(cancellationToken);

            var activeStatuses = new[] { "Active", "Current" };
            var suggestions = new List<ResourceSuggestionDto>();

            foreach (var emp in employees)
            {
                var activeAllocations = await _dbContext.ResourceAllocations
                    .Where(ra => ra.EmployeeId == emp.Id && !ra.IsDeleted && activeStatuses.Contains(ra.AllocationStatus))
                    .SumAsync(ra => ra.AllocationPercentage, cancellationToken);

                var availableCapacity = 100 - activeAllocations;

                var skillMatch = emp.EmployeeSkills?.Any() == true
                    ? Math.Min(100, emp.EmployeeSkills.Count * 25)
                    : 0;

                suggestions.Add(new ResourceSuggestionDto
                {
                    EmployeeId = emp.Id,
                    EmployeeName = emp.FullName,
                    SkillMatchPercentage = skillMatch,
                    AvailabilityPercentage = Math.Max(0, availableCapacity),
                    TotalAllocated = activeAllocations,
                    AvailableCapacity = Math.Max(0, availableCapacity),
                    ResourceStatus = activeAllocations >= 100 ? "Fully Allocated"
                        : activeAllocations > 0 ? "Partially Allocated"
                        : "Available"
                });
            }

            return suggestions
                .OrderByDescending(s => s.SkillMatchPercentage)
                .ThenByDescending(s => s.AvailabilityPercentage);
        }

        public async Task<IEnumerable<ResourceAvailabilityDto>> GetResourceAvailabilityAsync(CancellationToken cancellationToken = default)
        {
            var employees = await _dbContext.Employees
                .AsNoTracking()
                .Include(e => e.Designation)
                .Include(e => e.DepartmentType)
                .Include(e => e.Practice).ThenInclude(p => p.PracticeHead)
                .Include(e => e.EmployeeSkills).ThenInclude(es => es.Skill)
                .Where(e => !e.IsDeleted)
                .ToListAsync(cancellationToken);

            var activeStatuses = new[] { "Active", "Current" };
            var result = new List<ResourceAvailabilityDto>();

            foreach (var emp in employees)
            {
                var activeAllocations = await _dbContext.ResourceAllocations
                    .Where(ra => ra.EmployeeId == emp.Id && !ra.IsDeleted && activeStatuses.Contains(ra.AllocationStatus))
                    .ToListAsync(cancellationToken);

                var totalAllocated = activeAllocations.Sum(ra => ra.AllocationPercentage);
                var projectNames = activeAllocations.Select(ra => ra.Project?.ProjectName ?? "").Where(n => !string.IsNullOrEmpty(n)).ToList();
                var currentProject = projectNames.Count > 1 ? $"{projectNames[0]} (+{projectNames.Count - 1})" : projectNames.FirstOrDefault();
                var nextRelease = activeAllocations
                    .Where(ra => ra.EndDate.HasValue)
                    .OrderBy(ra => ra.EndDate)
                    .FirstOrDefault();

                result.Add(new ResourceAvailabilityDto
                {
                    EmployeeId = emp.Id,
                    EmployeeName = emp.FullName,
                    EmployeeCode = emp.EmployeeCode,
                    Designation = emp.Designation?.Name,
                    Practice = emp.Practice?.Name ?? "",
                    PracticeHead = emp.Practice?.PracticeHead?.FullName,
                    Department = emp.DepartmentType?.Name,
                    Skills = emp.EmployeeSkills != null ? string.Join(", ", emp.EmployeeSkills.Where(es => es.Skill != null).Select(es => es.Skill!.Name)) : null,
                    CurrentProject = currentProject,
                    TotalAllocated = totalAllocated,
                    AvailableCapacity = Math.Max(0, 100 - totalAllocated),
                    ResourceStatus = totalAllocated > 100 ? "Overallocated"
                        : totalAllocated >= 100 ? "Fully Allocated"
                        : totalAllocated > 0 ? "Partially Allocated"
                        : "Available",
                    NextAvailableDate = nextRelease?.EndDate
                });
            }

            return result.OrderBy(r => r.EmployeeName);
        }

        public async Task<IEnumerable<PracticeUtilizationDto>> GetPracticeUtilizationAsync(CancellationToken cancellationToken = default)
        {
            var practices = await _dbContext.Practices
                .AsNoTracking()
                .Where(p => !p.IsDeleted && p.IsActive)
                .Include(p => p.Employees.Where(e => !e.IsDeleted))
                .ToListAsync(cancellationToken);

            var result = new List<PracticeUtilizationDto>();

            foreach (var practice in practices)
            {
                var totalResources = practice.Employees?.Count ?? 0;
                var employeeIds = practice.Employees?.Select(e => e.Id).ToHashSet() ?? new HashSet<int>();

                var allocatedCount = 0;
                var benchCount = 0;

                if (employeeIds.Any())
                {
                    var activeStatuses = new[] { "Active", "Current" };
                    var allocatedIds = await _dbContext.ResourceAllocations
                        .Where(ra => employeeIds.Contains(ra.EmployeeId) && !ra.IsDeleted && activeStatuses.Contains(ra.AllocationStatus))
                        .Select(ra => ra.EmployeeId)
                        .Distinct()
                        .ToListAsync(cancellationToken);

                    allocatedCount = allocatedIds.Count;
                    benchCount = totalResources - allocatedCount;
                }

                var utilization = totalResources > 0
                    ? Math.Round((decimal)allocatedCount / totalResources * 100, 2)
                    : 0;

                result.Add(new PracticeUtilizationDto
                {
                    PracticeId = practice.Id,
                    PracticeName = practice.Name,
                    TotalResources = totalResources,
                    AllocatedResources = allocatedCount,
                    AvailableResources = totalResources - allocatedCount,
                    BenchResources = benchCount,
                    UtilizationPercentage = utilization
                });
            }

            return result;
        }
    }
}
