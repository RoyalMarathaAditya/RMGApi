using HRMS.Api.Models.RMG;

namespace HRMS.Api.Repositories.Interfaces.RMG
{
    public interface IResourceAllocationRepository
    {
        Task<IEnumerable<ResourceAllocation>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<ResourceAllocation?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<ResourceAllocation>> GetByEmployeeIdAsync(int employeeId, CancellationToken cancellationToken = default);
        Task<IEnumerable<ResourceAllocation>> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken = default);
        Task<ResourceAllocation> CreateAsync(ResourceAllocation allocation, CancellationToken cancellationToken = default);
        Task<ResourceAllocation> UpdateAsync(ResourceAllocation allocation, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<List<ResourceAllocation>> GetActiveByEmployeeIdAsync(int employeeId, CancellationToken cancellationToken = default);
    }
}
