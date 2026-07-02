using HRMS.Api.Models;

namespace HRMS.Api.Repositories.Interfaces
{
    public interface IColumnMappingRepository
    {
        Task<IEnumerable<ColumnMapping>> GetAllAsync(CancellationToken ct = default);
        Task<IEnumerable<ColumnMapping>> GetAllActiveAsync(CancellationToken ct = default);
        Task<ColumnMapping?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<ColumnMapping> CreateAsync(ColumnMapping mapping, CancellationToken ct = default);
        Task<ColumnMapping> UpdateAsync(ColumnMapping mapping, CancellationToken ct = default);
        Task DeleteAsync(ColumnMapping mapping, CancellationToken ct = default);
    }
}
