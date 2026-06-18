using HRMS.Api.DTOs.DesignationDtos;
using HRMS.Api.Models;

namespace HRMS.Api.Services.Interfaces
{
    public interface IDesignationService
    {
        Task<ApiResponse<IEnumerable<DesignationDto>>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<DesignationDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<ApiResponse<DesignationDto>> CreateAsync(CreateDesignationDto request, CancellationToken cancellationToken = default);

        Task<ApiResponse<DesignationDto>> UpdateAsync(int id, UpdateDesignationDto request, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}