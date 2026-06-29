using HRMS.Api.Models;

namespace HRMS.Api.Repositories.Interfaces
{
    public interface IAgeingBucketRepository
    {
        Task<IEnumerable<AgeingBucketMaster>> GetAllActiveAsync(CancellationToken ct = default);
    }
}
