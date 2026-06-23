using HRMS.Api.DTOs.ProjectAllocationDtos;

namespace HRMS.Api.Services.Interfaces
{
    public interface IProjectAllocationService
    {
        Task<IEnumerable<AllocationDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<AllocationDto>> GetByEmployeeIdAsync(int employeeId, CancellationToken cancellationToken = default);
        Task<IEnumerable<AllocationDto>> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken = default);
        Task<AllocationDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<AllocationDto> CreateAsync(CreateAllocationDto dto, CancellationToken cancellationToken = default);
        Task<AllocationDto> UpdateAsync(int id, UpdateAllocationDto dto, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> ReleaseResourceAsync(int id, CancellationToken cancellationToken = default);
    }
}
