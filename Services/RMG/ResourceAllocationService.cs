using AutoMapper;
using HRMS.Api.Data;
using HRMS.Api.DTOs.AllocationDtos;
using HRMS.Api.Models.RMG;
using HRMS.Api.Repositories.Interfaces.RMG;
using HRMS.Api.Services.Interfaces.RMG;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Services.RMG
{
    public class ResourceAllocationService : IResourceAllocationService
    {
        private readonly IResourceAllocationRepository _repository;
        private readonly IResourceAllocationHistoryRepository _historyRepository;
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public ResourceAllocationService(
            IResourceAllocationRepository repository,
            IResourceAllocationHistoryRepository historyRepository,
            AppDbContext dbContext,
            IMapper mapper)
        {
            _repository = repository;
            _historyRepository = historyRepository;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AllocationDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var allocations = await _repository.GetAllAsync(cancellationToken);
            var dtos = new List<AllocationDto>();
            foreach (var allocation in allocations)
            {
                dtos.Add(await ToDtoAsync(allocation, cancellationToken));
            }
            return dtos;
        }

        public async Task<AllocationDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var allocation = await _repository.GetByIdAsync(id, cancellationToken);
            return allocation is null ? null : await ToDtoAsync(allocation, cancellationToken);
        }

        public async Task<AllocationDto> CreateAsync(CreateAllocationDto dto, string userName, CancellationToken cancellationToken = default)
        {
            var employee = await _dbContext.Employees
                .FirstOrDefaultAsync(e => e.Id == dto.EmployeeId, cancellationToken)
                ?? throw new InvalidOperationException("Employee not found.");

            var totalAllocated = await GetTotalAllocatedAsync(dto.EmployeeId, null, cancellationToken);
            if (totalAllocated + dto.AllocationPercentage > 100)
                throw new InvalidOperationException($"Total allocation cannot exceed 100%. Current allocation: {totalAllocated}%.");

            var allocation = new ResourceAllocation
            {
                EmployeeId = dto.EmployeeId,
                ProjectId = dto.ProjectId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                AllocationPercentage = dto.AllocationPercentage,
                AllocationStatus = dto.AllocationStatus ?? "Planned",
                Notes = dto.Notes,
                CreatedBy = userName
            };

            var created = await _repository.CreateAsync(allocation, cancellationToken);
            await SaveHistoryAsync(created, "Created", userName, dto.Notes, cancellationToken);
            return await ToDtoAsync(created, cancellationToken);
        }

        public async Task<AllocationDto?> UpdateAsync(int id, UpdateAllocationDto dto, string userName, CancellationToken cancellationToken = default)
        {
            var allocation = await _repository.GetByIdAsync(id, cancellationToken);
            if (allocation is null) return null;

            var oldProjectId = allocation.ProjectId;
            var oldPercentage = allocation.AllocationPercentage;
            var oldStatus = allocation.AllocationStatus;

            if (dto.ProjectId.HasValue) allocation.ProjectId = dto.ProjectId.Value;
            if (dto.StartDate.HasValue) allocation.StartDate = dto.StartDate.Value;
            if (dto.EndDate.HasValue) allocation.EndDate = dto.EndDate;
            if (dto.AllocationPercentage.HasValue) allocation.AllocationPercentage = dto.AllocationPercentage.Value;
            if (!string.IsNullOrEmpty(dto.AllocationStatus)) allocation.AllocationStatus = dto.AllocationStatus;
            if (dto.Notes is not null) allocation.Notes = dto.Notes;
            allocation.ModifiedBy = userName;
            allocation.ModifiedOn = DateTime.UtcNow;

            var totalAllocated = await GetTotalAllocatedAsync(allocation.EmployeeId, id, cancellationToken);
            if (totalAllocated + allocation.AllocationPercentage > 100)
                throw new InvalidOperationException($"Total allocation cannot exceed 100%. Current allocation excluding this: {totalAllocated}%.");

            var updated = await _repository.UpdateAsync(allocation, cancellationToken);

            var historyNotes = $"Updated: {(dto.AllocationPercentage.HasValue ? $"{oldPercentage}% → {allocation.AllocationPercentage}%" : "")} {(dto.AllocationStatus != null ? $"Status: {oldStatus} → {allocation.AllocationStatus}" : "")}".Trim();
            if (string.IsNullOrEmpty(historyNotes)) historyNotes = dto.Notes ?? "Updated allocation";

            await SaveHistoryAsync(updated, "Updated", userName, historyNotes, cancellationToken);
            return await ToDtoAsync(updated, cancellationToken);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var allocation = await _repository.GetByIdAsync(id, cancellationToken);
            if (allocation is null) return false;

            await _repository.DeleteAsync(id, cancellationToken);

            if (allocation.Employee is not null)
            {
                await SaveHistoryAsync(allocation, "Deleted", allocation.ModifiedBy ?? "system", "Allocation deleted", cancellationToken);
            }

            return true;
        }

        public async Task<IEnumerable<AllocationHistoryDto>> GetHistoryAsync(int allocationId, CancellationToken cancellationToken = default)
        {
            var history = await _historyRepository.GetByAllocationIdAsync(allocationId, cancellationToken);
            var result = new List<AllocationHistoryDto>();

            foreach (var h in history)
            {
                string? oldProject = null;
                string? newProject = null;
                if (h.OldProjectId.HasValue)
                {
                    var proj = await _dbContext.Projects.FindAsync(new object[] { h.OldProjectId.Value }, cancellationToken);
                    oldProject = proj?.ProjectName;
                }
                if (h.NewProjectId.HasValue)
                {
                    var proj = await _dbContext.Projects.FindAsync(new object[] { h.NewProjectId.Value }, cancellationToken);
                    newProject = proj?.ProjectName;
                }

                result.Add(new AllocationHistoryDto
                {
                    Id = h.Id,
                    AllocationId = h.ResourceAllocationId,
                    OldProject = oldProject,
                    NewProject = newProject,
                    OldAllocationPercentage = h.OldAllocationPercentage,
                    NewAllocationPercentage = h.NewAllocationPercentage,
                    OldStatus = h.OldAllocationStatus,
                    NewStatus = h.NewAllocationStatus,
                    ChangeType = h.ChangeType,
                    ModifiedBy = h.ModifiedBy,
                    ModifiedDate = h.ModifiedDate,
                    Remarks = h.Remarks
                });
            }

            return result;
        }

        public async Task<IEnumerable<CalendarViewDto>> GetCalendarDataAsync(CancellationToken cancellationToken = default)
        {
            var allocations = await _dbContext.ResourceAllocations
                .AsNoTracking()
                .Include(ra => ra.Employee)
                .Include(ra => ra.Project)
                .Where(ra => !ra.IsDeleted)
                .ToListAsync(cancellationToken);

            return allocations.Select(ra => new CalendarViewDto
            {
                AllocationId = ra.Id,
                EmployeeId = ra.EmployeeId,
                EmployeeName = ra.Employee?.FullName ?? "",
                ProjectName = ra.Project?.ProjectName ?? "",
                StartDate = ra.StartDate,
                EndDate = ra.EndDate,
                AllocationPercentage = ra.AllocationPercentage,
                AllocationStatus = ra.AllocationStatus,
                ColorCode = ra.AllocationStatus switch
                {
                    "Active" => "#4caf50",
                    "Planned" => "#2196f3",
                    "Completed" => "#9e9e9e",
                    "Released" => "#ff9800",
                    "Cancelled" => "#f44336",
                    _ => "#9e9e9e"
                }
            });
        }

        public async Task<IEnumerable<TimelineViewDto>> GetTimelineDataAsync(CancellationToken cancellationToken = default)
        {
            var allocations = await _dbContext.ResourceAllocations
                .AsNoTracking()
                .Include(ra => ra.Employee)
                .Include(ra => ra.Project)
                .Where(ra => !ra.IsDeleted)
                .OrderBy(ra => ra.StartDate)
                .ToListAsync(cancellationToken);

            var grouped = allocations.GroupBy(ra => new { ra.ProjectId, ProjectName = ra.Project?.ProjectName ?? "" });

            return grouped.Select(g => new TimelineViewDto
            {
                ProjectId = g.Key.ProjectId,
                ProjectName = g.Key.ProjectName,
                Employees = g.Select(ra => new TimelineEmployeeDto
                {
                    EmployeeId = ra.EmployeeId,
                    EmployeeName = ra.Employee?.FullName ?? "",
                    StartDate = ra.StartDate,
                    EndDate = ra.EndDate,
                    AllocationPercentage = ra.AllocationPercentage,
                    AllocationStatus = ra.AllocationStatus
                }).ToList()
            });
        }

        private async Task<decimal> GetTotalAllocatedAsync(int employeeId, int? excludeAllocationId = null, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.ResourceAllocations
                .Where(ra => ra.EmployeeId == employeeId && !ra.IsDeleted && ra.AllocationStatus != "Cancelled" && ra.AllocationStatus != "Released");

            if (excludeAllocationId.HasValue)
                query = query.Where(ra => ra.Id != excludeAllocationId.Value);

            return await query.SumAsync(ra => ra.AllocationPercentage, cancellationToken);
        }

        private async Task SaveHistoryAsync(ResourceAllocation allocation, string changeType, string userName, string? remarks, CancellationToken cancellationToken = default)
        {
            var history = new ResourceAllocationHistory
            {
                ResourceAllocationId = allocation.Id,
                EmployeeId = allocation.EmployeeId,
                ChangeType = changeType,
                ModifiedBy = userName,
                ModifiedDate = DateTime.UtcNow,
                Remarks = remarks
            };

            await _historyRepository.CreateAsync(history, cancellationToken);
        }

        private async Task<AllocationDto> ToDtoAsync(ResourceAllocation allocation, CancellationToken cancellationToken = default)
        {
            var employee = allocation.Employee ?? await _dbContext.Employees
                .Include(e => e.Designation)
                .Include(e => e.Practice).ThenInclude(p => p.PracticeHead)
                .Include(e => e.EmployeeSkills).ThenInclude(es => es.Skill)
                .FirstOrDefaultAsync(e => e.Id == allocation.EmployeeId, cancellationToken);

            var totalAllocated = await GetTotalAllocatedAsync(allocation.EmployeeId, allocation.Id, cancellationToken);
            var myAllocation = totalAllocated + allocation.AllocationPercentage;
            var available = 100 - myAllocation;

            var resourceStatus = allocation.AllocationStatus switch
            {
                "Active" when myAllocation >= 100 => "Fully Allocated",
                "Active" when myAllocation > 100 => "Overallocated",
                "Active" => "Partially Allocated",
                "Planned" => "Available",
                _ => allocation.AllocationStatus
            };

            return new AllocationDto
            {
                Id = allocation.Id,
                EmployeeId = allocation.EmployeeId,
                EmployeeName = employee?.FullName ?? "",
                EmployeeCode = employee?.EmployeeCode ?? "",
                Designation = employee?.Designation?.Name,
                Practice = employee?.Practice?.Name ?? "",
                PracticeHead = employee?.Practice?.PracticeHead?.FullName,
                Skills = employee?.EmployeeSkills != null ? string.Join(", ", employee.EmployeeSkills.Where(es => es.Skill != null).Select(es => es.Skill!.Name)) : null,
                ProjectId = allocation.ProjectId,
                ProjectName = allocation.Project?.ProjectName ?? "",
                StartDate = allocation.StartDate,
                EndDate = allocation.EndDate,
                AllocationPercentage = allocation.AllocationPercentage,
                AllocationStatus = allocation.AllocationStatus,
                Notes = allocation.Notes,
                TotalAllocated = myAllocation,
                AvailableCapacity = Math.Max(0, available),
                ResourceStatus = resourceStatus
            };
        }
    }
}
