using HRMS.Api.Data;
using HRMS.Api.DTOs.DashboardDtos;
using HRMS.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly AppDbContext _dbContext;

        public DashboardService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ResourceSummaryDto> GetResourceSummaryAsync(CancellationToken cancellationToken = default)
        {
            var total = await _dbContext.Employees.CountAsync(cancellationToken);
            var active = await _dbContext.Employees.CountAsync(e => e.StatusId == Guid.Parse("10000000-0000-0000-0000-000000000001"), cancellationToken);

            var currentStatusId = Guid.Parse("90000000-0000-0000-0000-000000000001");
            var billableStatusId = Guid.Parse("A0000000-0000-0000-0000-000000000001");

            var billable = await _dbContext.ProjectAllocations
                .Where(pa => pa.AllocationStatusId == currentStatusId && pa.EmployeeProjectStatusId == billableStatusId)
                .Select(pa => pa.EmployeeId).Distinct().CountAsync(cancellationToken);

            var utilized = await _dbContext.ProjectAllocations
                .Where(pa => pa.AllocationStatusId == currentStatusId && pa.IsUtilized)
                .Select(pa => pa.EmployeeId).Distinct().CountAsync(cancellationToken);

            var bench = total - utilized;

            return new ResourceSummaryDto
            {
                TotalEmployees = total,
                ActiveEmployees = active,
                BenchEmployees = Math.Max(bench, 0),
                BillableEmployees = billable,
                UtilizedPercentage = active > 0 ? Math.Round((decimal)utilized / active * 100, 2) : 0
            };
        }

        public async Task<IEnumerable<PracticeSummaryDto>> GetPracticeSummaryAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Employees
                .GroupBy(e => new { e.PracticeId, e.Practice.Name })
                .Select(g => new PracticeSummaryDto
                {
                    PracticeId = 0,
                    PracticeName = g.Key.Name,
                    EmployeeCount = g.Count()
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<AllocationSummaryDto> GetAllocationSummaryAsync(CancellationToken cancellationToken = default)
        {
            var currentStatusId = Guid.Parse("90000000-0000-0000-0000-000000000001");

            var currentAllocations = await _dbContext.ProjectAllocations
                .CountAsync(pa => pa.AllocationStatusId == currentStatusId, cancellationToken);

            var upcomingReleases = await _dbContext.ProjectAllocations
                .CountAsync(pa => pa.AllocationEndDate.HasValue
                    && pa.AllocationEndDate.Value <= DateTime.UtcNow.AddDays(30)
                    && pa.AllocationEndDate.Value >= DateTime.UtcNow, cancellationToken);

            var longLeaveStatusId = Guid.Parse("A0000000-0000-0000-0000-000000000009");
            var longLeaves = await _dbContext.EmployeeLeaves
                .CountAsync(el => el.LeaveTypeId == Guid.Parse("60000000-0000-0000-0000-000000000003"), cancellationToken);

            var pipEmployees = await _dbContext.PIPs.CountAsync(p => p.Status == "Active", cancellationToken);

            return new AllocationSummaryDto
            {
                CurrentAllocations = currentAllocations,
                UpcomingReleases = upcomingReleases,
                LongLeaveEmployees = longLeaves,
                PipEmployees = pipEmployees
            };
        }

        public async Task<IEnumerable<object>> GetUpcomingReleasesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProjectAllocations
                .AsNoTracking()
                .Include(pa => pa.Employee)
                .Include(pa => pa.Project)
                .Where(pa => pa.AllocationEndDate.HasValue
                    && pa.AllocationEndDate.Value <= DateTime.UtcNow.AddDays(30)
                    && pa.AllocationEndDate.Value >= DateTime.UtcNow)
                .Select(pa => new
                {
                    pa.Id,
                    EmployeeName = pa.Employee.FullName,
                    ProjectName = pa.Project.ProjectName,
                    ReleaseDate = pa.AllocationEndDate,
                    pa.AllocationPercentage
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<decimal> GetUtilizationPercentageAsync(CancellationToken cancellationToken = default)
        {
            var active = await _dbContext.Employees.CountAsync(e => e.StatusId == Guid.Parse("10000000-0000-0000-0000-000000000001"), cancellationToken);
            var currentStatusId = Guid.Parse("90000000-0000-0000-0000-000000000001");
            var utilized = await _dbContext.ProjectAllocations
                .Where(pa => pa.AllocationStatusId == currentStatusId && pa.IsUtilized)
                .Select(pa => pa.EmployeeId).Distinct().CountAsync(cancellationToken);

            return active > 0 ? Math.Round((decimal)utilized / active * 100, 2) : 0;
        }

        public async Task<IEnumerable<object>> GetBenchResourcesAsync(CancellationToken cancellationToken = default)
        {
            var currentStatusId = Guid.Parse("90000000-0000-0000-0000-000000000001");

            var allocatedEmployeeIds = await _dbContext.ProjectAllocations
                .Where(pa => pa.AllocationStatusId == currentStatusId)
                .Select(pa => pa.EmployeeId)
                .Distinct()
                .ToListAsync(cancellationToken);

            var partialAllocations = await _dbContext.ProjectAllocations
                .Where(pa => pa.AllocationStatusId == currentStatusId)
                .GroupBy(pa => pa.EmployeeId)
                .Select(g => new { EmployeeId = g.Key, TotalAllocation = g.Sum(pa => pa.AllocationPercentage) })
                .ToListAsync(cancellationToken);

            var partialEmployeeIds = partialAllocations.Where(x => x.TotalAllocation < 100).Select(x => x.EmployeeId).ToHashSet();

            var allBenchIds = new HashSet<int>();
            allBenchIds.UnionWith(allocatedEmployeeIds.Where(id => partialEmployeeIds.Contains(id)));

            return await _dbContext.Employees
                .AsNoTracking()
                .Include(e => e.Practice)
                .Where(e => !allocatedEmployeeIds.Contains(e.Id) || partialEmployeeIds.Contains(e.Id))
                .Select(e => new
                {
                    EmployeeId = e.Id,
                    EmployeeName = e.FullName,
                    PracticeName = e.Practice.Name,
                    CurrentAllocation = partialAllocations.Where(x => x.EmployeeId == e.Id).Select(x => x.TotalAllocation).FirstOrDefault(),
                    AvailablePercentage = 100 - (partialAllocations.Where(x => x.EmployeeId == e.Id).Select(x => x.TotalAllocation).FirstOrDefault())
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<object>> GetResourceAvailabilityAsync(CancellationToken cancellationToken = default)
        {
            var currentStatusId = Guid.Parse("90000000-0000-0000-0000-000000000001");

            return await _dbContext.Employees
                .AsNoTracking()
                .Include(e => e.Practice)
                .Select(e => new
                {
                    EmployeeId = e.Id,
                    EmployeeName = e.FullName,
                    PracticeName = e.Practice.Name,
                    CurrentAllocation = _dbContext.ProjectAllocations
                        .Where(pa => pa.EmployeeId == e.Id && pa.AllocationStatusId == currentStatusId)
                        .Sum(pa => pa.AllocationPercentage),
                    NextAssignmentDate = _dbContext.ProjectAllocations
                        .Where(pa => pa.EmployeeId == e.Id && pa.AllocationStatusId == currentStatusId)
                        .Min(pa => pa.NextAssignmentDate),
                    UpcomingReleaseDate = _dbContext.ProjectAllocations
                        .Where(pa => pa.EmployeeId == e.Id && pa.AllocationEndDate.HasValue)
                        .Max(pa => pa.AllocationEndDate)
                })
                .ToListAsync(cancellationToken);
        }
    }
}
