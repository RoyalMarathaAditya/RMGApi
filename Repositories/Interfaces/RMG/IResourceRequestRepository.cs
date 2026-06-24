using HRMS.Api.Models.RMG;

namespace HRMS.Api.Repositories.Interfaces.RMG
{
    public interface IResourceRequestRepository
    {
        Task<IEnumerable<ResourceRequest>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<ResourceRequest?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<ResourceRequest> CreateAsync(ResourceRequest request, CancellationToken cancellationToken = default);
        Task<ResourceRequest> UpdateAsync(ResourceRequest request, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
