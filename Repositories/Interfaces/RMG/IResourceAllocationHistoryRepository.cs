using HRMS.Api.Models.RMG;

namespace HRMS.Api.Repositories.Interfaces.RMG
{
    public interface IResourceAllocationHistoryRepository
    {
        Task<IEnumerable<ResourceAllocationHistory>> GetByAllocationIdAsync(int allocationId, CancellationToken cancellationToken = default);
        Task<IEnumerable<ResourceAllocationHistory>> GetByEmployeeIdAsync(int employeeId, CancellationToken cancellationToken = default);
        Task<ResourceAllocationHistory> CreateAsync(ResourceAllocationHistory history, CancellationToken cancellationToken = default);
    }
}
