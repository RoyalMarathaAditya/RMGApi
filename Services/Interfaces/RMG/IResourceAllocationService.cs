using HRMS.Api.DTOs.AllocationDtos;

namespace HRMS.Api.Services.Interfaces.RMG
{
    public interface IResourceAllocationService
    {
        Task<IEnumerable<AllocationDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<AllocationDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<AllocationDto> CreateAsync(CreateAllocationDto dto, string userName, CancellationToken cancellationToken = default);
        Task<AllocationDto?> UpdateAsync(int id, UpdateAllocationDto dto, string userName, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<AllocationHistoryDto>> GetHistoryAsync(int allocationId, CancellationToken cancellationToken = default);
        Task<IEnumerable<CalendarViewDto>> GetCalendarDataAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<TimelineViewDto>> GetTimelineDataAsync(CancellationToken cancellationToken = default);

        Task<EmployeeAllocationDto> GetEmployeeAllocationsAsync(int employeeId, CancellationToken cancellationToken = default);
        Task<ProjectAllocationDto> AddProjectAllocationAsync(AddProjectAllocationDto dto, string userName, CancellationToken cancellationToken = default);
        Task<ProjectAllocationDto?> UpdateProjectAllocationAsync(int allocationId, UpdateProjectAllocationDto dto, string userName, CancellationToken cancellationToken = default);
        Task<bool> DeleteProjectAllocationAsync(int allocationId, CancellationToken cancellationToken = default);
        Task<EmployeeCapacitySummaryDto> GetEmployeeCapacitySummaryAsync(int employeeId, CancellationToken cancellationToken = default);
        Task<EmployeeResourceDetailsDto> GetEmployeeDetailsAsync(int employeeId, CancellationToken cancellationToken = default);
    }

    public class AllocationHistoryDto
    {
        public int Id { get; set; }
        public int AllocationId { get; set; }
        public string? OldProject { get; set; }
        public string? NewProject { get; set; }
        public decimal? OldAllocationPercentage { get; set; }
        public decimal? NewAllocationPercentage { get; set; }
        public string? OldStatus { get; set; }
        public string? NewStatus { get; set; }
        public string ChangeType { get; set; } = string.Empty;
        public string ModifiedBy { get; set; } = string.Empty;
        public DateTime ModifiedDate { get; set; }
        public string? Remarks { get; set; }
    }
}
