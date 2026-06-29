using HRMS.Api.Models;

namespace HRMS.Api.Repositories.Interfaces
{
    public interface IOnboardingStatusRepository
    {
        Task<IEnumerable<OnboardingStatusMaster>> GetAllActiveAsync(CancellationToken ct = default);
    }
}
