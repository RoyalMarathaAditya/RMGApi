using HRMS.Api.Models;

namespace HRMS.Api.Repositories.Interfaces
{
    public interface IColumnValueMappingRepository
    {
        Task<IEnumerable<ColumnValueMapping>> GetAllAsync(CancellationToken ct = default);
        Task<IEnumerable<ColumnValueMapping>> GetActiveByTargetPropertyAsync(string targetProperty, CancellationToken ct = default);
        Task<IEnumerable<ColumnValueMapping>> GetAllActiveAsync(CancellationToken ct = default);
        Task<ColumnValueMapping?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<ColumnValueMapping> CreateAsync(ColumnValueMapping mapping, CancellationToken ct = default);
        Task<ColumnValueMapping> UpdateAsync(ColumnValueMapping mapping, CancellationToken ct = default);
        Task DeleteAsync(ColumnValueMapping mapping, CancellationToken ct = default);
    }
}
