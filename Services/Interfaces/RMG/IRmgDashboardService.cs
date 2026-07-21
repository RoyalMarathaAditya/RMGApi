using HRMS.Api.DTOs.AllocationDtos;
using HRMS.Api.DTOs.Common;
using HRMS.Api.DTOs.RmgDashboardDtos;

namespace HRMS.Api.Services.Interfaces.RMG
{
    public interface IRmgDashboardService
    {
        Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default);
        Task<PaginatedResponse<DashboardGridDto>> GetGridDataAsync(
            DashboardFilterDto? filter = null,
            PagedFilterDto? paging = null,
            CancellationToken cancellationToken = default);
        Task<IEnumerable<ResourceSuggestionDto>> GetSuitableResourcesAsync(int projectId, CancellationToken cancellationToken = default);
        Task<IEnumerable<ResourceAvailabilityDto>> GetResourceAvailabilityAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<PracticeUtilizationDto>> GetPracticeUtilizationAsync(CancellationToken cancellationToken = default);
        Task InvalidateDashboardCacheAsync();
    }
}
