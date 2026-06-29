using HRMS.Api.Models;

namespace HRMS.Api.Repositories.Interfaces
{
    public interface ICurrentBillingStatusRepository
    {
        Task<IEnumerable<CurrentBillingStatusMaster>> GetAllActiveAsync(CancellationToken ct = default);
    }
}
