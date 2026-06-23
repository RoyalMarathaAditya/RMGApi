using HRMS.Api.Models;

namespace HRMS.Api.Repositories.Interfaces
{
    public interface IDesignationRepository
    {
        Task<IEnumerable<DesignationMaster>> GetAllAsync(CancellationToken ct = default);
        Task<IEnumerable<DesignationMaster>> GetActiveAsync(CancellationToken ct = default);
        Task<DesignationMaster?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<DesignationMaster> CreateAsync(DesignationMaster designation, CancellationToken ct = default);
        Task<DesignationMaster> UpdateAsync(DesignationMaster designation, CancellationToken ct = default);
        Task DeleteAsync(DesignationMaster designation, CancellationToken ct = default);
        Task<bool> ExistsAsync(string name, CancellationToken ct = default);
        Task<bool> CodeExistsAsync(string code, CancellationToken ct = default);
    }
}