using HRMS.Api.DTOs.MasterDtos;
using HRMS.Api.Models;

namespace HRMS.Api.Services.Interfaces
{
    public interface IBillingBucketService
    {
        Task<ApiResponse<IEnumerable<MasterDto>>> GetAllActiveAsync(CancellationToken ct = default);
    }
}
