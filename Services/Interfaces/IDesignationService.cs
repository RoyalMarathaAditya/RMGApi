using HRMS.Api.DTOs;
using HRMS.Api.Models;

namespace HRMS.Api.Services.Interfaces
{
    public interface IDesignationService
    {
        Task<ApiResponse<IEnumerable<DesignationDto>>> GetAllAsync(CancellationToken ct = default);
        Task<ApiResponse<IEnumerable<DesignationDto>>> GetActiveAsync(CancellationToken ct = default);
        Task<ApiResponse<DesignationDto>> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<ApiResponse<DesignationDto>> CreateAsync(CreateDesignationDto request, CancellationToken ct = default);
        Task<ApiResponse<DesignationDto>> UpdateAsync(Guid id, UpdateDesignationDto request, CancellationToken ct = default);
        Task<ApiResponse<bool>> DeleteAsync(Guid id, CancellationToken ct = default);
    }
}