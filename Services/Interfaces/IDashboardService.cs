using HRMS.Api.DTOs.DashboardDtos;

namespace HRMS.Api.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<IEnumerable<PracticeSummaryDto>> GetPracticeSummaryAsync(CancellationToken cancellationToken = default);
    }
}
