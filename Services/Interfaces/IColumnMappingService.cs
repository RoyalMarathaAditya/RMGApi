using HRMS.Api.DTOs;
using HRMS.Api.Models;

namespace HRMS.Api.Services.Interfaces
{
    public interface IColumnMappingService
    {
        Task<ApiResponse<IEnumerable<ColumnMappingDto>>> GetAllAsync(CancellationToken ct = default);
        Task<ApiResponse<ColumnMappingDto>> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<ApiResponse<ColumnMappingDto>> CreateAsync(CreateColumnMappingDto request, CancellationToken ct = default);
        Task<ApiResponse<ColumnMappingDto>> UpdateAsync(Guid id, UpdateColumnMappingDto request, CancellationToken ct = default);
        Task<ApiResponse<bool>> DeleteAsync(Guid id, CancellationToken ct = default);
    }
}
