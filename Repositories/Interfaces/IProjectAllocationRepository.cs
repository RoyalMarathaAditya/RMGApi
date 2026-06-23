using HRMS.Api.Models;

namespace HRMS.Api.Repositories.Interfaces
{
    public interface IProjectAllocationRepository
    {
        Task<IEnumerable<ProjectAllocation>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<ProjectAllocation>> GetByEmployeeIdAsync(int employeeId, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProjectAllocation>> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken = default);
        Task<ProjectAllocation?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<ProjectAllocation> CreateAsync(ProjectAllocation allocation, CancellationToken cancellationToken = default);
        Task<ProjectAllocation> UpdateAsync(ProjectAllocation allocation, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
