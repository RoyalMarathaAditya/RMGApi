using System.Diagnostics;
using AutoMapper;
using HRMS.Api.Data;
using HRMS.Api.DTOs.AllocationDtos;
using HRMS.Api.Models;
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
        private readonly ILogger<ResourceAllocationService> _logger;

        public ResourceAllocationService(
            IResourceAllocationRepository repository,
            IResourceAllocationHistoryRepository historyRepository,
            AppDbContext dbContext,
            IMapper mapper,
            ILogger<ResourceAllocationService> logger)
        {
            _repository = repository;
            _historyRepository = historyRepository;
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
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
            _logger.LogInformation("Creating allocation: EmployeeId={EmployeeId} ProjectId={ProjectId} User={User}",
                dto.EmployeeId, dto.ProjectId, userName);
            var sw = Stopwatch.StartNew();
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
                AllocationStatus = dto.EndDate.HasValue && dto.EndDate.Value.Date >= DateTime.Today ? "Current" : "History",
                AllocationType = dto.AllocationType,
                BillableStatus = dto.BillableStatus,
                Notes = dto.Notes,
                CreatedBy = userName
            };

            var created = await _repository.CreateAsync(allocation, cancellationToken);
            sw.Stop();
            _logger.LogInformation("Allocation {AllocationId} created in {ElapsedMs}ms", created.Id, sw.ElapsedMilliseconds);
            await SaveHistoryAsync(created, "Created", userName, dto.Notes, cancellationToken);
            return await ToDtoAsync(created, cancellationToken);
        }

        public async Task<AllocationDto?> UpdateAsync(int id, UpdateAllocationDto dto, string userName, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Updating allocation {AllocationId} by user {User}", id, userName);
            var sw = Stopwatch.StartNew();
            var allocation = await _repository.GetByIdAsync(id, cancellationToken);
            if (allocation is null) return null;

            var oldProjectId = allocation.ProjectId;
            var oldPercentage = allocation.AllocationPercentage;
            var oldStatus = allocation.AllocationStatus;

            if (dto.ProjectId.HasValue) allocation.ProjectId = dto.ProjectId.Value;
            if (dto.StartDate.HasValue) allocation.StartDate = dto.StartDate.Value;
            if (dto.EndDate.HasValue)
            {
                allocation.EndDate = dto.EndDate;
                allocation.AllocationStatus = dto.EndDate.Value.Date >= DateTime.Today ? "Current" : "History";
            }
            if (dto.AllocationPercentage.HasValue) allocation.AllocationPercentage = dto.AllocationPercentage.Value;
            if (dto.AllocationType is not null) allocation.AllocationType = dto.AllocationType;
            if (dto.BillableStatus is not null) allocation.BillableStatus = dto.BillableStatus;
            if (dto.Notes is not null) allocation.Notes = dto.Notes;
            allocation.ModifiedBy = userName;
            allocation.ModifiedOn = DateTime.UtcNow;

            var totalAllocated = await GetTotalAllocatedAsync(allocation.EmployeeId, id, cancellationToken);
            if (totalAllocated + allocation.AllocationPercentage > 100)
                throw new InvalidOperationException($"Total allocation cannot exceed 100%. Current allocation excluding this: {totalAllocated}%.");

            var updated = await _repository.UpdateAsync(allocation, cancellationToken);
            sw.Stop();
            _logger.LogInformation("Allocation {AllocationId} updated in {ElapsedMs}ms", id, sw.ElapsedMilliseconds);

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

        public async Task<EmployeeAllocationDto> GetEmployeeAllocationsAsync(int employeeId, CancellationToken cancellationToken = default)
        {
            var employee = await _dbContext.Employees
                .AsNoTracking()
                .Include(e => e.Designation)
                .Include(e => e.Practice)
                .Include(e => e.EmployeeSkills).ThenInclude(es => es.Skill)
                .FirstOrDefaultAsync(e => e.Id == employeeId, cancellationToken)
                ?? throw new InvalidOperationException("Employee not found.");

            var activeAllocations = await _repository.GetActiveByEmployeeIdAsync(employeeId, cancellationToken);
            var totalAllocated = activeAllocations.Sum(a => a.AllocationPercentage);
            var available = 100 - totalAllocated;

            var resourceStatus = totalAllocated >= 100 ? "Fully Allocated"
                : totalAllocated > 0 ? "Partially Allocated"
                : "Available";

            if (totalAllocated > 100) resourceStatus = "Overallocated";

            return new EmployeeAllocationDto
            {
                EmployeeId = employee.Id,
                EmployeeName = employee.FullName,
                EmployeeCode = employee.EmployeeCode,
                Designation = employee.Designation?.Name,
                Practice = employee.Practice?.Name ?? "",
                Skills = employee.EmployeeSkills != null
                    ? string.Join(", ", employee.EmployeeSkills.Where(es => es.Skill != null).Select(es => es.Skill!.Name))
                    : null,
                PrimarySkill = employee.EmployeeSkills?.FirstOrDefault(es => es.Skill != null)?.Skill?.Name,
                TotalExperience = employee.ExperienceYears ?? CalculateTotalExperience(employee),
                RelevantExperience = employee.RelevantExperience,
                IsActive = !employee.IsDeleted,
                ReportingManagerId = employee.ReportingManagerId,
                CurrentUtilization = totalAllocated,
                AvailableCapacity = Math.Max(0, available),
                ResourceStatus = resourceStatus,
                Allocations = activeAllocations.Select(a => new ProjectAllocationDto
                {
                    Id = a.Id,
                    ProjectId = a.ProjectId,
                    ProjectName = a.Project?.ProjectName ?? "",
                    ClientId = a.ClientId ?? a.Project?.ClientId,
                    ClientName = a.Client?.Name ?? a.Project?.Client?.Name,
                    ProjectStatusId = a.ProjectStatusId,
                    StatusId = a.StatusId,
                    ProbableNextAssignmentId = a.ProbableNextAssignmentId,
                    ProbableNextAssignmentDate = a.ProbableNextAssignmentDate,
                    BillableDateProbabilityId = a.BillableDateProbabilityId,
                    CurrentBillingStatusId = a.CurrentBillingStatusId,
                    BillingBucketId = a.BillingBucketId,
                    AgeingBucketId = a.AgeingBucketId,
                    StartDate = a.StartDate,
                    EndDate = a.EndDate,
                    AllocationPercentage = a.AllocationPercentage,
                    BillableStatus = a.BillableStatus,
                    AllocationType = a.AllocationType,
                    AllocationStatus = a.AllocationStatus,
                    ActionItem = a.ActionItem,
                    Remarks = a.Remarks
                }).ToList()
            };
        }

        public async Task<ProjectAllocationDto> AddProjectAllocationAsync(AddProjectAllocationDto dto, string userName, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Adding project allocation: EmployeeId={EmployeeId} ProjectId={ProjectId} User={User}",
                dto.EmployeeId, dto.ProjectId, userName);
            var sw = Stopwatch.StartNew();
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
                ClientId = dto.ClientId,
                ProjectStatusId = dto.ProjectStatusId,
                StatusId = dto.StatusId,
                ProbableNextAssignmentId = dto.ProbableNextAssignmentId,
                ProbableNextAssignmentDate = dto.ProbableNextAssignmentDate,
                BillableDateProbabilityId = dto.BillableDateProbabilityId,
                CurrentBillingStatusId = dto.CurrentBillingStatusId,
                BillingBucketId = dto.BillingBucketId,
                AgeingBucketId = dto.AgeingBucketId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                AllocationPercentage = dto.AllocationPercentage,
                AllocationStatus = dto.EndDate.HasValue && dto.EndDate.Value.Date >= DateTime.Today ? "Current" : "History",
                AllocationType = dto.AllocationType,
                BillableStatus = dto.BillableStatus,
                ActionItem = dto.ActionItem,
                Remarks = dto.Remarks,
                CreatedBy = userName
            };

            var created = await _repository.CreateAsync(allocation, cancellationToken);
            sw.Stop();
            _logger.LogInformation("Project allocation {AllocationId} created in {ElapsedMs}ms", created.Id, sw.ElapsedMilliseconds);
            await SaveHistoryAsync(created, "Created", userName, null, cancellationToken);
            return await ToProjectAllocationDtoAsync(created, cancellationToken);
        }

        public async Task<ProjectAllocationDto?> UpdateProjectAllocationAsync(int allocationId, UpdateProjectAllocationDto dto, string userName, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Updating project allocation {AllocationId} by user {User}", allocationId, userName);
            var sw = Stopwatch.StartNew();
            var allocation = await _repository.GetByIdAsync(allocationId, cancellationToken);
            if (allocation is null) return null;

            var oldPercentage = allocation.AllocationPercentage;

            if (dto.ProjectId.HasValue) allocation.ProjectId = dto.ProjectId.Value;
            if (dto.ClientId.HasValue) allocation.ClientId = dto.ClientId.Value;
            if (dto.ProjectStatusId.HasValue) allocation.ProjectStatusId = dto.ProjectStatusId.Value;
            if (dto.StatusId.HasValue) allocation.StatusId = dto.StatusId.Value;
            if (dto.ProbableNextAssignmentId.HasValue) allocation.ProbableNextAssignmentId = dto.ProbableNextAssignmentId.Value;
            if (dto.ProbableNextAssignmentDate.HasValue) allocation.ProbableNextAssignmentDate = dto.ProbableNextAssignmentDate;
            if (dto.BillableDateProbabilityId.HasValue) allocation.BillableDateProbabilityId = dto.BillableDateProbabilityId.Value;
            if (dto.CurrentBillingStatusId.HasValue) allocation.CurrentBillingStatusId = dto.CurrentBillingStatusId.Value;
            if (dto.BillingBucketId.HasValue) allocation.BillingBucketId = dto.BillingBucketId.Value;
            if (dto.AgeingBucketId.HasValue) allocation.AgeingBucketId = dto.AgeingBucketId.Value;
            if (dto.StartDate.HasValue) allocation.StartDate = dto.StartDate.Value;
            if (dto.EndDate.HasValue)
            {
                allocation.EndDate = dto.EndDate;
                allocation.AllocationStatus = dto.EndDate.Value.Date >= DateTime.Today ? "Current" : "History";
            }
            if (dto.AllocationPercentage.HasValue) allocation.AllocationPercentage = dto.AllocationPercentage.Value;
            if (dto.AllocationType is not null) allocation.AllocationType = dto.AllocationType;
            if (dto.BillableStatus is not null) allocation.BillableStatus = dto.BillableStatus;
            if (dto.ActionItem is not null) allocation.ActionItem = dto.ActionItem;
            if (dto.Remarks is not null) allocation.Remarks = dto.Remarks;
            allocation.ModifiedBy = userName;
            allocation.ModifiedOn = DateTime.UtcNow;

            var totalAllocated = await GetTotalAllocatedAsync(allocation.EmployeeId, allocationId, cancellationToken);
            if (totalAllocated + allocation.AllocationPercentage > 100)
                throw new InvalidOperationException($"Total allocation cannot exceed 100%. Current allocation excluding this: {totalAllocated}%.");

            var updated = await _repository.UpdateAsync(allocation, cancellationToken);
            sw.Stop();
            _logger.LogInformation("Project allocation {AllocationId} updated in {ElapsedMs}ms", allocationId, sw.ElapsedMilliseconds);
            await SaveHistoryAsync(updated, "Updated", userName, null, cancellationToken);
            return await ToProjectAllocationDtoAsync(updated, cancellationToken);
        }

        public async Task<bool> DeleteProjectAllocationAsync(int allocationId, CancellationToken cancellationToken = default)
        {
            return await DeleteAsync(allocationId, cancellationToken);
        }

        public async Task<EmployeeCapacitySummaryDto> GetEmployeeCapacitySummaryAsync(int employeeId, CancellationToken cancellationToken = default)
        {
            var activeAllocations = await _repository.GetActiveByEmployeeIdAsync(employeeId, cancellationToken);
            var totalAllocated = activeAllocations.Sum(a => a.AllocationPercentage);
            var available = 100 - totalAllocated;

            var resourceStatus = totalAllocated >= 100 ? "Fully Allocated"
                : totalAllocated > 0 ? "Partially Allocated"
                : "Available";

            if (totalAllocated > 100) resourceStatus = "Overallocated";

            return new EmployeeCapacitySummaryDto
            {
                TotalCapacity = 100,
                AllocatedCapacity = totalAllocated,
                AvailableCapacity = Math.Max(0, available),
                ResourceStatus = resourceStatus
            };
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
                    "Current" => "#4caf50",
                    "Planned" => "#2196f3",
                    "Completed" => "#9e9e9e",
                    "History" => "#9e9e9e",
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

        public async Task<bool> UpdateEmployeeDetailsAsync(int employeeId, UpdateEmployeeDetailsDto dto, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Updating employee details for EmployeeId={EmployeeId}", employeeId);
            var sw = Stopwatch.StartNew();
            var employee = await _dbContext.Employees
                .Include(e => e.EmployeeSkills)
                .FirstOrDefaultAsync(e => e.Id == employeeId, cancellationToken);

            if (employee is null)
            {
                _logger.LogWarning("Employee {EmployeeId} not found for details update", employeeId);
                return false;
            }

            employee.RelevantExperience = dto.ExperienceInNV;
            employee.ReportingManagerId = dto.ProjectManagerId;
            employee.IsDeleted = !dto.IsActive;
            employee.ModifiedOn = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync(cancellationToken);
            sw.Stop();
            _logger.LogInformation("Employee {EmployeeId} details updated in {ElapsedMs}ms", employeeId, sw.ElapsedMilliseconds);
            return true;
        }

        public async Task<EmployeeResourceDetailsDto> GetEmployeeDetailsAsync(int employeeId, CancellationToken cancellationToken = default)
        {
            var employee = await _dbContext.Employees
                .AsNoTracking()
                .Include(e => e.Designation)
                .Include(e => e.Practice).ThenInclude(p => p.PracticeHead)
                .Include(e => e.Location)
                .Include(e => e.EmploymentType)
                .Include(e => e.EmployeeStatus)
                .Include(e => e.ReportingManager)
                .Include(e => e.EmployeeSkills).ThenInclude(es => es.Skill)
                .FirstOrDefaultAsync(e => e.Id == employeeId, cancellationToken)
                ?? throw new InvalidOperationException("Employee not found.");

            var activeAllocations = await _dbContext.ResourceAllocations
                .AsNoTracking()
                .Include(ra => ra.Project).ThenInclude(p => p.Client)
                .Where(ra => ra.EmployeeId == employeeId && !ra.IsDeleted && ra.AllocationStatus != "Cancelled" && ra.AllocationStatus != "Released" && ra.AllocationStatus != "History")
                .ToListAsync(cancellationToken);

            var totalAllocated = activeAllocations.Sum(a => a.AllocationPercentage);
            var isUtilised = activeAllocations.Any();
            var isBillable = activeAllocations.Any(a => a.BillableStatus == "Billable");

            var priorExp = employee.PriorExperience ?? 0;
            var totalExperience = employee.ExperienceYears ?? priorExp + (employee.RelevantExperience ?? 0);
            var nvExperience = totalExperience - priorExp;
            if (nvExperience < 0) nvExperience = 0;

            var skills = employee.EmployeeSkills?.Where(es => es.Skill != null).Select(es => es.Skill!.Name).ToList() ?? new List<string>();
            var primarySkill = skills.FirstOrDefault();
            var skillString = skills.Any() ? string.Join(", ", skills) : null;

            var today = DateTime.UtcNow.Date;

            return new EmployeeResourceDetailsDto
            {
                EmployeeId = employee.Id,
                EmployeeCode = employee.EmployeeCode,
                EmployeeName = employee.FullName,
                Email = employee.Email,
                Role = employee.Designation?.Name,
                Practice = employee.Practice?.Name,
                SubPractice = null,
                PrimarySkill = primarySkill,
                Skill = skillString,
                Active = !employee.IsDeleted,
                Location = employee.Location?.Name,
                L1Manager = employee.ReportingManager?.FullName,
                PracticeHead = employee.Practice?.PracticeHead?.FullName,
                DOJ = employee.DOJ,

                PriorExperience = employee.PriorExperience ?? 0,
                NVExperience = Math.Round(nvExperience, 1),
                TotalExperience = Math.Round(totalExperience, 1),
                ExperienceRange = GetExperienceRange(totalExperience),

                FteConsultant = employee.EmploymentType?.Name,
                Utilised = isUtilised ? "Yes" : "No",
                Billable = isBillable ? "Yes" : "No",
                Status = employee.EmployeeStatus?.Name ?? (employee.IsDeleted ? "Inactive" : "Active"),

                ProjectAllocations = activeAllocations.Select(a =>
                {
                    var startDate = a.StartDate;
                    var endDate = a.EndDate ?? today;
                    var durationDays = (endDate - startDate).Days;
                    var ageingDays = (today - startDate).Days;

                    return new ProjectAllocationDetailDto
                    {
                        ProjectCode = a.Project?.ProjectCode,
                        Client = a.Project?.Client?.Name,
                        Project = a.Project?.ProjectName,
                        ProjectStatus = a.BillableStatus,
                        StartDate = a.StartDate,
                        EndDate = a.EndDate,
                        AllocationPercentage = a.AllocationPercentage,
                        BillablePercentage = a.BillableStatus == "Billable" ? a.AllocationPercentage : 0,
                        Engineering = employee.Engineering.HasValue ? (employee.Engineering.Value ? "Yes" : "No") : null,
                        DurationInProject = $"{durationDays} Days",
                        Ageing = $"{Math.Max(0, ageingDays)} Days",
                        Remarks = a.Notes
                    };
                }).ToList()
            };
        }

        private static string GetExperienceRange(decimal totalExperience)
        {
            if (totalExperience <= 2) return "0 - 2 Years";
            if (totalExperience <= 6) return "3 - 6 Years";
            if (totalExperience <= 9) return "6 - 9 Years";
            if (totalExperience <= 12) return "9 - 12 Years";
            return "More than 12 Years";
        }

        private async Task<decimal> GetTotalAllocatedAsync(int employeeId, int? excludeAllocationId = null, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.ResourceAllocations
                .Where(ra => ra.EmployeeId == employeeId && !ra.IsDeleted && ra.AllocationStatus != "Cancelled" && ra.AllocationStatus != "Released" && ra.AllocationStatus != "History");

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
                "Current" when myAllocation >= 100 => "Fully Allocated",
                "Current" when myAllocation > 100 => "Overallocated",
                "Current" => "Partially Allocated",
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
                AllocationType = allocation.AllocationType,
                BillableStatus = allocation.BillableStatus,
                Notes = allocation.Notes,
                TotalAllocated = myAllocation,
                AvailableCapacity = Math.Max(0, available),
                ResourceStatus = resourceStatus
            };
        }

        private async Task<ProjectAllocationDto> ToProjectAllocationDtoAsync(ResourceAllocation allocation, CancellationToken cancellationToken = default)
        {
            var project = allocation.Project ?? await _dbContext.Projects.Include(p => p.Client).FirstOrDefaultAsync(p => p.Id == allocation.ProjectId, cancellationToken);
            return new ProjectAllocationDto
            {
                Id = allocation.Id,
                ProjectId = allocation.ProjectId,
                ProjectName = project?.ProjectName ?? "",
                ClientId = allocation.ClientId ?? project?.ClientId,
                ClientName = allocation.Client?.Name ?? project?.Client?.Name,
                ProjectStatusId = allocation.ProjectStatusId,
                StatusId = allocation.StatusId,
                ProbableNextAssignmentId = allocation.ProbableNextAssignmentId,
                ProbableNextAssignmentDate = allocation.ProbableNextAssignmentDate,
                BillableDateProbabilityId = allocation.BillableDateProbabilityId,
                CurrentBillingStatusId = allocation.CurrentBillingStatusId,
                BillingBucketId = allocation.BillingBucketId,
                AgeingBucketId = allocation.AgeingBucketId,
                StartDate = allocation.StartDate,
                EndDate = allocation.EndDate,
                AllocationPercentage = allocation.AllocationPercentage,
                BillableStatus = allocation.BillableStatus,
                AllocationType = allocation.AllocationType,
                AllocationStatus = allocation.AllocationStatus,
                ActionItem = allocation.ActionItem,
                Remarks = allocation.Remarks
            };
        }

        private static decimal CalculateTotalExperience(Employee employee)
        {
            var doj = employee.DOJ;
            var priorExperience = employee.PriorExperience ?? 0;
            var yearsSinceDoj = (decimal)(DateTime.UtcNow - doj).TotalDays / 365.25m;
            return Math.Round(yearsSinceDoj + priorExperience, 1);
        }
    }
}
