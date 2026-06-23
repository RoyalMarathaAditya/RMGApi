using HRMS.Api.DTOs.DashboardDtos;

namespace HRMS.Api.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<ResourceSummaryDto> GetResourceSummaryAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<PracticeSummaryDto>> GetPracticeSummaryAsync(CancellationToken cancellationToken = default);
        Task<AllocationSummaryDto> GetAllocationSummaryAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<object>> GetUpcomingReleasesAsync(CancellationToken cancellationToken = default);
        Task<decimal> GetUtilizationPercentageAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<object>> GetBenchResourcesAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<object>> GetResourceAvailabilityAsync(CancellationToken cancellationToken = default);
    }
}
