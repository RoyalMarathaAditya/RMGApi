using System.Diagnostics;
using HRMS.Api.Data;
using HRMS.Api.DTOs.AllocationDtos;
using HRMS.Api.DTOs.Common;
using HRMS.Api.DTOs.RmgDashboardDtos;
using HRMS.Api.Models;
using HRMS.Api.Services.Interfaces.RMG;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace HRMS.Api.Services.RMG
{
    public class RmgDashboardService : IRmgDashboardService
    {
        private readonly AppDbContext _dbContext;
        private readonly IDistributedCache _cache;
        private readonly ILogger<RmgDashboardService> _logger;
        private static readonly string[] ActiveStatuses = { "Active", "Current" };
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

        public RmgDashboardService(AppDbContext dbContext, IDistributedCache cache, ILogger<RmgDashboardService> logger)
        {
            _dbContext = dbContext;
            _cache = cache;
            _logger = logger;
        }

        private async Task<T?> GetCachedAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
        {
            try
            {
                var cached = await _cache.GetStringAsync(key, cancellationToken);
                if (cached != null)
                    return JsonSerializer.Deserialize<T>(cached);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Cache read failed for key {CacheKey}", key);
            }
            return null;
        }

        private async Task SetCachedAsync<T>(string key, T value, CancellationToken cancellationToken = default)
        {
            try
            {
                await _cache.SetStringAsync(key, JsonSerializer.Serialize(value),
                    new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = CacheDuration }, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Cache write failed for key {CacheKey}", key);
            }
        }

        private async Task RemoveCachedAsync(string key)
        {
            try
            {
                await _cache.RemoveAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Cache remove failed for key {CacheKey}", key);
            }
        }

        public async Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default)
        {
            var sw = Stopwatch.StartNew();

            var cacheKey = "dash:summary";
            var cached = await GetCachedAsync<DashboardSummaryDto>(cacheKey, cancellationToken);
            if (cached != null)
            {
                _logger.LogInformation("DASHBOARD SUMMARY cache HIT");
                sw.Stop();
                _logger.LogInformation("DASHBOARD SUMMARY [{ElapsedMs} ms] CACHED", sw.ElapsedMilliseconds);
                return cached;
            }

            _logger.LogInformation("DASHBOARD SUMMARY cache MISS - querying DB");

            var activeStatuses = ActiveStatuses;

            var totalEmployees = await _dbContext.Employees
                .AsNoTracking()
                .CountAsync(e => !e.IsDeleted, cancellationToken);

            var totalPractices = await _dbContext.Practices
                .AsNoTracking()
                .CountAsync(p => !p.IsDeleted && p.IsActive, cancellationToken);

            var employeeTotals = await _dbContext.ResourceAllocations
                .AsNoTracking()
                .Where(ra => !ra.IsDeleted && activeStatuses.Contains(ra.AllocationStatus))
                .GroupBy(ra => ra.EmployeeId)
                .Select(g => new AllocationTotal { EmployeeId = g.Key, Total = g.Sum(ra => ra.AllocationPercentage) })
                .ToListAsync(cancellationToken);

            var onLeaveCount = await _dbContext.EmployeeLeaves
                .AsNoTracking()
                .CountAsync(el => el.FromDate <= DateTime.UtcNow && el.ToDate >= DateTime.UtcNow, cancellationToken);

            var practices = await _dbContext.Practices
                .AsNoTracking()
                .Where(p => !p.IsDeleted && p.IsActive)
                .Select(p => new PracticeCount { Id = p.Id, EmployeeCount = p.Employees.Count(e => !e.IsDeleted) })
                .ToListAsync(cancellationToken);

            var employeeIdsWithAllocations = employeeTotals.Select(a => a.EmployeeId).ToHashSet();
            var availableCount = employeeTotals.Count(a => a.Total == 0)
                + (totalEmployees - employeeIdsWithAllocations.Count);
            var allocatedCount = employeeTotals.Count(a => a.Total > 0 && a.Total < 100);
            var fullyAllocatedCount = employeeTotals.Count(a => a.Total >= 100);
            var utilizedEmployees = employeeTotals.Count;
            var totalPracticeResources = practices.Sum(p => p.EmployeeCount);

            var summary = new DashboardSummaryDto
            {
                TotalEmployees = totalEmployees,
                TotalPractices = totalPractices,
                AvailableResources = availableCount,
                PartiallyAllocatedResources = allocatedCount,
                FullyAllocatedResources = fullyAllocatedCount,
                TotalAllocatedResources = allocatedCount + fullyAllocatedCount,
                OverallocatedResources = employeeTotals.Count(a => a.Total > 100),
                BenchResources = availableCount,
                ResourcesOnLeave = onLeaveCount,
                PracticeUtilizationPercentage = totalPracticeResources > 0
                    ? Math.Round((decimal)utilizedEmployees / totalPracticeResources * 100, 2)
                    : 0
            };

            await SetCachedAsync(cacheKey, summary, cancellationToken);

            sw.Stop();
            _logger.LogInformation("DASHBOARD SUMMARY [{ElapsedMs} ms] DB", sw.ElapsedMilliseconds);
            return summary;
        }

        public async Task<PaginatedResponse<DashboardGridDto>> GetGridDataAsync(
            DashboardFilterDto? filter = null,
            PagedFilterDto? paging = null,
            CancellationToken cancellationToken = default)
        {
            var sw = Stopwatch.StartNew();
            paging ??= new PagedFilterDto();
            filter ??= new DashboardFilterDto();
            var now = DateTime.UtcNow;

            var baseQuery = BuildEmployeeBaseQuery(filter);
            var totalCount = await baseQuery.CountAsync(cancellationToken);

            var sortedQuery = ApplySorting(baseQuery, paging);
            var skip = (paging.Page - 1) * paging.PageSize;

            var employees = await sortedQuery
                .Skip(skip)
                .Take(paging.PageSize)
                .Select(e => new EmployeeGridRaw
                {
                    Id = e.Id,
                    FullName = e.FullName,
                    EmployeeCode = e.EmployeeCode,
                    DOJ = e.DOJ,
                    PriorExperience = e.PriorExperience,
                    DesignationName = e.Designation != null ? e.Designation.Name : null,
                    DepartmentName = e.DepartmentType != null ? e.DepartmentType.Name : null,
                    PracticeName = e.Practice.Name,
                    PracticeHeadName = e.Practice.PracticeHead != null ? e.Practice.PracticeHead.FullName : null,
                    SubPracticeName = e.SubPractice != null ? e.SubPractice.Name : null,
                    IsActive = e.EmployeeStatus == null || e.EmployeeStatus.Name != "Inactive",
                    OnLeave = e.EmployeeLeaves.Any(el => el.FromDate <= now && el.ToDate >= now)
                })
                .ToListAsync(cancellationToken);

            var employeeIds = employees.Select(e => e.Id).ToList();

            var skills = await _dbContext.EmployeeSkills
                .AsNoTracking()
                .Where(es => employeeIds.Contains(es.EmployeeId) && es.Skill != null)
                .Select(es => new SkillInfo { EmployeeId = es.EmployeeId, SkillName = es.Skill!.Name })
                .ToListAsync(cancellationToken);

            var allocations = await _dbContext.ResourceAllocations
                .AsNoTracking()
                .Where(ra => employeeIds.Contains(ra.EmployeeId) && !ra.IsDeleted && ActiveStatuses.Contains(ra.AllocationStatus))
                .Select(ra => new AllocationInfo
                {
                    EmployeeId = ra.EmployeeId,
                    AllocationPercentage = ra.AllocationPercentage,
                    ProjectName = ra.Project != null ? ra.Project.ProjectName : null
                })
                .ToListAsync(cancellationToken);

            var skillsByEmployee = skills
                .GroupBy(es => es.EmployeeId)
                .ToDictionary(g => g.Key, g => string.Join(", ", g.Select(x => x.SkillName)));

            var allocationsByEmployee = allocations
                .GroupBy(a => a.EmployeeId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var result = new List<DashboardGridDto>();

            foreach (var emp in employees)
            {
                var hasAllocs = allocationsByEmployee.TryGetValue(emp.Id, out var empAllocs);
                var totalAllocated = hasAllocs ? empAllocs!.Sum(a => a.AllocationPercentage) : 0m;
                var projectNames = hasAllocs
                    ? empAllocs!.Select(a => a.ProjectName).Where(n => !string.IsNullOrEmpty(n)).Cast<string>().ToList()
                    : new List<string>();
                var currentProject = projectNames.Count > 1
                    ? $"{projectNames[0]} (+{projectNames.Count - 1})"
                    : projectNames.FirstOrDefault();

                var resourceStatus = !emp.IsActive ? "Inactive"
                    : emp.OnLeave ? "On Leave"
                    : totalAllocated > 100 ? "Overallocated"
                    : totalAllocated >= 100 ? "Fully Allocated"
                    : totalAllocated > 0 ? "Partially Allocated"
                    : "Available";

                if (!string.IsNullOrEmpty(filter.ResourceStatus) && resourceStatus != filter.ResourceStatus)
                    continue;

                var totalExperience = Math.Round(
                    ((decimal)(now - emp.DOJ).TotalDays / 365.25m) + (emp.PriorExperience ?? 0), 1);

                result.Add(new DashboardGridDto
                {
                    EmployeeId = emp.Id,
                    EmployeeName = emp.FullName,
                    EmployeeCode = emp.EmployeeCode,
                    Designation = emp.DesignationName,
                    Department = emp.DepartmentName,
                    Practice = emp.PracticeName ?? "",
                    PracticeHead = emp.PracticeHeadName,
                    SubPractice = emp.SubPracticeName,
                    TotalExperience = totalExperience,
                    Skills = skillsByEmployee.GetValueOrDefault(emp.Id),
                    CurrentProject = currentProject,
                    Projects = projectNames,
                    AllocationPercentage = totalAllocated,
                    AvailableCapacity = Math.Max(0, 100 - totalAllocated),
                    ResourceStatus = resourceStatus,
                    AllocationStatus = "Active",
                    IsActive = emp.IsActive
                });
            }

            sw.Stop();
            _logger.LogInformation("GRID [{ElapsedMs} ms] page={Page} size={Size} total={Total}",
                sw.ElapsedMilliseconds, paging.Page, paging.PageSize, totalCount);

            return new PaginatedResponse<DashboardGridDto>
            {
                Items = result,
                TotalCount = totalCount,
                Page = paging.Page,
                PageSize = paging.PageSize
            };
        }

        public async Task<IEnumerable<ResourceSuggestionDto>> GetSuitableResourcesAsync(int projectId, CancellationToken cancellationToken = default)
        {
            var sw = Stopwatch.StartNew();

            var projectExists = await _dbContext.Projects
                .AsNoTracking()
                .AnyAsync(p => p.Id == projectId, cancellationToken);

            if (!projectExists) return Enumerable.Empty<ResourceSuggestionDto>();

            var activeStatuses = ActiveStatuses;

            var employees = await _dbContext.Employees
                .AsNoTracking()
                .Where(e => !e.IsDeleted)
                .Select(e => new
                {
                    e.Id,
                    e.FullName,
                    SkillCount = e.EmployeeSkills.Count(es => es.Skill != null)
                })
                .ToListAsync(cancellationToken);

            var employeeIds = employees.Select(e => e.Id).ToList();
            var allocationTotals = await _dbContext.ResourceAllocations
                .AsNoTracking()
                .Where(ra => employeeIds.Contains(ra.EmployeeId) && !ra.IsDeleted && activeStatuses.Contains(ra.AllocationStatus))
                .GroupBy(ra => ra.EmployeeId)
                .Select(g => new { EmployeeId = g.Key, Total = g.Sum(ra => ra.AllocationPercentage) })
                .ToListAsync(cancellationToken);

            var allocByEmployee = allocationTotals.ToDictionary(a => a.EmployeeId, a => a.Total);

            var result = employees
                .Select(emp =>
                {
                    var total = allocByEmployee.GetValueOrDefault(emp.Id, 0m);
                    return new ResourceSuggestionDto
                    {
                        EmployeeId = emp.Id,
                        EmployeeName = emp.FullName,
                        SkillMatchPercentage = emp.SkillCount > 0 ? Math.Min(100, emp.SkillCount * 25) : 0,
                        AvailabilityPercentage = Math.Max(0, 100 - total),
                        TotalAllocated = total,
                        AvailableCapacity = Math.Max(0, 100 - total),
                        ResourceStatus = total >= 100 ? "Fully Allocated"
                            : total > 0 ? "Partially Allocated"
                            : "Available"
                    };
                })
                .OrderByDescending(s => s.SkillMatchPercentage)
                .ThenByDescending(s => s.AvailabilityPercentage)
                .ToList();

            sw.Stop();
            _logger.LogInformation("SUITABLE RESOURCES [{ElapsedMs} ms] project={ProjectId}",
                sw.ElapsedMilliseconds, projectId);
            return result;
        }

        public async Task<IEnumerable<ResourceAvailabilityDto>> GetResourceAvailabilityAsync(CancellationToken cancellationToken = default)
        {
            var sw = Stopwatch.StartNew();
            var now = DateTime.UtcNow;
            var activeStatuses = ActiveStatuses;

            var employees = await _dbContext.Employees
                .AsNoTracking()
                .Where(e => !e.IsDeleted)
                .Select(e => new EmployeeAvailabilityRaw
                {
                    Id = e.Id,
                    FullName = e.FullName,
                    EmployeeCode = e.EmployeeCode,
                    DesignationName = e.Designation != null ? e.Designation.Name : null,
                    DepartmentName = e.DepartmentType != null ? e.DepartmentType.Name : null,
                    PracticeName = e.Practice.Name,
                    PracticeHeadName = e.Practice.PracticeHead != null ? e.Practice.PracticeHead.FullName : null
                })
                .ToListAsync(cancellationToken);

            var employeeIds = employees.Select(e => e.Id).ToList();

            var skills = await _dbContext.EmployeeSkills
                .AsNoTracking()
                .Where(es => employeeIds.Contains(es.EmployeeId) && es.Skill != null)
                .Select(es => new SkillInfo { EmployeeId = es.EmployeeId, SkillName = es.Skill!.Name })
                .ToListAsync(cancellationToken);

            var allocations = await _dbContext.ResourceAllocations
                .AsNoTracking()
                .Where(ra => employeeIds.Contains(ra.EmployeeId) && !ra.IsDeleted && activeStatuses.Contains(ra.AllocationStatus))
                .Select(ra => new AllocationInfoFull
                {
                    EmployeeId = ra.EmployeeId,
                    AllocationPercentage = ra.AllocationPercentage,
                    ProjectName = ra.Project != null ? ra.Project.ProjectName : null,
                    EndDate = ra.EndDate
                })
                .ToListAsync(cancellationToken);

            var skillsByEmployee = skills
                .GroupBy(es => es.EmployeeId)
                .ToDictionary(g => g.Key, g => string.Join(", ", g.Select(x => x.SkillName)));

            var allocationsByEmployee = allocations
                .GroupBy(a => a.EmployeeId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var result = employees.Select(emp =>
            {
                var hasAllocs = allocationsByEmployee.TryGetValue(emp.Id, out var empAllocs);
                var totalAllocated = hasAllocs ? empAllocs!.Sum(a => a.AllocationPercentage) : 0m;
                var projectNames = hasAllocs
                    ? empAllocs!.Select(a => a.ProjectName).Where(n => !string.IsNullOrEmpty(n)).Cast<string>().ToList()
                    : new List<string>();
                var currentProject = projectNames.Count > 1
                    ? $"{projectNames[0]} (+{projectNames.Count - 1})"
                    : projectNames.FirstOrDefault();
                var nextRelease = hasAllocs
                    ? empAllocs!.Where(a => a.EndDate.HasValue).OrderBy(a => a.EndDate).FirstOrDefault()
                    : null;

                return new ResourceAvailabilityDto
                {
                    EmployeeId = emp.Id,
                    EmployeeName = emp.FullName,
                    EmployeeCode = emp.EmployeeCode,
                    Designation = emp.DesignationName,
                    Practice = emp.PracticeName ?? "",
                    PracticeHead = emp.PracticeHeadName,
                    Department = emp.DepartmentName,
                    Skills = skillsByEmployee.GetValueOrDefault(emp.Id),
                    CurrentProject = currentProject,
                    TotalAllocated = totalAllocated,
                    AvailableCapacity = Math.Max(0, 100 - totalAllocated),
                    ResourceStatus = totalAllocated > 100 ? "Overallocated"
                        : totalAllocated >= 100 ? "Fully Allocated"
                        : totalAllocated > 0 ? "Partially Allocated"
                        : "Available",
                    NextAvailableDate = nextRelease?.EndDate
                };
            })
            .OrderBy(r => r.EmployeeName)
            .ToList();

            sw.Stop();
            _logger.LogInformation("RESOURCE AVAILABILITY [{ElapsedMs} ms]", sw.ElapsedMilliseconds);
            return result;
        }

        public async Task<IEnumerable<PracticeUtilizationDto>> GetPracticeUtilizationAsync(CancellationToken cancellationToken = default)
        {
            var sw = Stopwatch.StartNew();

            var cacheKey = "dash:practice-util";
            var cached = await GetCachedAsync<List<PracticeUtilizationDto>>(cacheKey, cancellationToken);
            if (cached != null)
            {
                _logger.LogInformation("PRACTICE UTIL cache HIT");
                return cached;
            }

            _logger.LogInformation("PRACTICE UTIL cache MISS");
            var activeStatuses = ActiveStatuses;

            var practicesWithTotals = await _dbContext.Practices
                .AsNoTracking()
                .Where(p => !p.IsDeleted && p.IsActive)
                .Select(p => new
                {
                    PracticeId = p.Id,
                    PracticeName = p.Name,
                    TotalResources = p.Employees.Count(e => !e.IsDeleted)
                })
                .ToListAsync(cancellationToken);

            var allPracticeEmployeeIds = await _dbContext.Employees
                .AsNoTracking()
                .Where(e => !e.IsDeleted)
                .Select(e => new { e.Id, e.PracticeId })
                .ToListAsync(cancellationToken);

            var allocatedEmployeeIds = await _dbContext.ResourceAllocations
                .AsNoTracking()
                .Where(ra => !ra.IsDeleted && activeStatuses.Contains(ra.AllocationStatus))
                .Select(ra => ra.EmployeeId)
                .Distinct()
                .ToListAsync(cancellationToken);

            var allocatedSet = allocatedEmployeeIds.ToHashSet();

            var employeePracticeLookup = allPracticeEmployeeIds
                .GroupBy(e => e.PracticeId)
                .ToDictionary(g => g.Key, g => g.Select(e => e.Id).ToHashSet());

            var practiceData = practicesWithTotals.Select(p =>
            {
                var empIds = employeePracticeLookup.GetValueOrDefault(p.PracticeId) ?? new HashSet<int>();
                var allocatedCount = empIds.Count(id => allocatedSet.Contains(id));
                return new
                {
                    p.PracticeId,
                    p.PracticeName,
                    p.TotalResources,
                    AllocatedCount = allocatedCount
                };
            }).ToList();

            var result = practiceData.Select(p => new PracticeUtilizationDto
            {
                PracticeId = p.PracticeId,
                PracticeName = p.PracticeName,
                TotalResources = p.TotalResources,
                AllocatedResources = p.AllocatedCount,
                AvailableResources = p.TotalResources - p.AllocatedCount,
                BenchResources = p.TotalResources - p.AllocatedCount,
                UtilizationPercentage = p.TotalResources > 0
                    ? Math.Round((decimal)p.AllocatedCount / p.TotalResources * 100, 2)
                    : 0
            }).ToList();

            await SetCachedAsync(cacheKey, result, cancellationToken);

            sw.Stop();
            _logger.LogInformation("PRACTICE UTIL [{ElapsedMs} ms]", sw.ElapsedMilliseconds);
            return result;
        }

        public async Task InvalidateDashboardCacheAsync()
        {
            _logger.LogInformation("Invalidating dashboard cache...");
            await Task.WhenAll(
                RemoveCachedAsync("dash:summary"),
                RemoveCachedAsync("dash:practice-util"),
                RemoveCachedAsync("dash:grid")
            );
        }

        private IQueryable<Employee> BuildEmployeeBaseQuery(DashboardFilterDto filter)
        {
            var query = _dbContext.Employees
                .AsNoTracking()
                .Where(e => !e.IsDeleted);

            if (!string.IsNullOrEmpty(filter.SearchTerm))
            {
                var term = filter.SearchTerm.ToLower();
                query = query.Where(e =>
                    e.FullName.ToLower().Contains(term) ||
                    e.EmployeeCode.ToLower().Contains(term));
            }

            if (!string.IsNullOrEmpty(filter.EmployeeName))
                query = query.Where(e => e.FullName.Contains(filter.EmployeeName));
            if (!string.IsNullOrEmpty(filter.EmployeeCode))
                query = query.Where(e => e.EmployeeCode.Contains(filter.EmployeeCode));
            if (!string.IsNullOrEmpty(filter.Practice))
                query = query.Where(e => e.Practice.Name == filter.Practice);
            if (!string.IsNullOrEmpty(filter.Designation))
                query = query.Where(e => e.Designation != null && e.Designation.Name == filter.Designation);
            if (!string.IsNullOrEmpty(filter.Department))
                query = query.Where(e => e.DepartmentType != null && e.DepartmentType.Name == filter.Department);
            if (!string.IsNullOrEmpty(filter.PracticeHead))
                query = query.Where(e => e.Practice.PracticeHead.FullName.Contains(filter.PracticeHead));

            return query;
        }

        private static IQueryable<Employee> ApplySorting(IQueryable<Employee> query, PagedFilterDto paging)
        {
            if (string.IsNullOrEmpty(paging.SortField))
                return query.OrderBy(e => e.FullName);

            var desc = string.Equals(paging.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

            return paging.SortField.ToLower() switch
            {
                "employeename" => desc ? query.OrderByDescending(e => e.FullName) : query.OrderBy(e => e.FullName),
                "employeecode" => desc ? query.OrderByDescending(e => e.EmployeeCode) : query.OrderBy(e => e.EmployeeCode),
                _ => query.OrderBy(e => e.FullName)
            };
        }

        private class AllocationTotal
        {
            public int EmployeeId { get; set; }
            public decimal Total { get; set; }
        }

        private class PracticeCount
        {
            public Guid Id { get; set; }
            public int EmployeeCount { get; set; }
        }

        private class EmployeeGridRaw
        {
            public int Id { get; set; }
            public string FullName { get; set; } = string.Empty;
            public string EmployeeCode { get; set; } = string.Empty;
            public DateTime DOJ { get; set; }
            public decimal? PriorExperience { get; set; }
            public string? DesignationName { get; set; }
            public string? DepartmentName { get; set; }
            public string? PracticeName { get; set; }
            public string? PracticeHeadName { get; set; }
            public string? SubPracticeName { get; set; }
            public bool IsActive { get; set; }
            public bool OnLeave { get; set; }
        }

        private class EmployeeAvailabilityRaw
        {
            public int Id { get; set; }
            public string FullName { get; set; } = string.Empty;
            public string EmployeeCode { get; set; } = string.Empty;
            public string? DesignationName { get; set; }
            public string? DepartmentName { get; set; }
            public string? PracticeName { get; set; }
            public string? PracticeHeadName { get; set; }
        }

        private class SkillInfo
        {
            public int EmployeeId { get; set; }
            public string SkillName { get; set; } = string.Empty;
        }

        private class AllocationInfo
        {
            public int EmployeeId { get; set; }
            public decimal AllocationPercentage { get; set; }
            public string? ProjectName { get; set; }
        }

        private class AllocationInfoFull
        {
            public int EmployeeId { get; set; }
            public decimal AllocationPercentage { get; set; }
            public string? ProjectName { get; set; }
            public DateTime? EndDate { get; set; }
        }
    }
}
