using HRMS.Api.Models;

namespace HRMS.Api.Repositories.Interfaces
{
    public interface IBillingBucketRepository
    {
        Task<IEnumerable<BillingBucketMaster>> GetAllActiveAsync(CancellationToken ct = default);
    }
}
