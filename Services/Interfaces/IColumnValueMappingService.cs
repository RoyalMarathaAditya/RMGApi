using HRMS.Api.DTOs;
using HRMS.Api.Models;

namespace HRMS.Api.Services.Interfaces
{
    public interface IColumnValueMappingService
    {
        Task<ApiResponse<IEnumerable<ColumnValueMappingDto>>> GetAllAsync(CancellationToken ct = default);
        Task<ApiResponse<ColumnValueMappingDto>> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<ApiResponse<ColumnValueMappingDto>> CreateAsync(CreateColumnValueMappingDto request, CancellationToken ct = default);
        Task<ApiResponse<ColumnValueMappingDto>> UpdateAsync(Guid id, UpdateColumnValueMappingDto request, CancellationToken ct = default);
        Task<ApiResponse<bool>> DeleteAsync(Guid id, CancellationToken ct = default);
    }
}
