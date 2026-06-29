using HRMS.Api.Models;

namespace HRMS.Api.Repositories.Interfaces
{
    public interface IBillableDateProbabilityRepository
    {
        Task<IEnumerable<BillableDateProbabilityMaster>> GetAllActiveAsync(CancellationToken ct = default);
    }
}
