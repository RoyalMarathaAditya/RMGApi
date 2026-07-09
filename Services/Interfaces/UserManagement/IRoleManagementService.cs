using HRMS.Api.DTOs.UserDtos;

namespace HRMS.Api.Services.Interfaces.UserManagement
{
    public interface IRoleManagementService
    {
        Task<List<RoleDto>> GetRolesAsync(CancellationToken cancellationToken = default);
        Task<RoleDto?> GetRoleByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ApiResponse<RoleDto>> CreateRoleAsync(CreateRoleDto dto, CancellationToken cancellationToken = default);
        Task<ApiResponse<RoleDto>> UpdateRoleAsync(Guid id, UpdateRoleDto dto, CancellationToken cancellationToken = default);
        Task<ApiResponse<bool>> DeleteRoleAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ApiResponse<bool>> ActivateRoleAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ApiResponse<bool>> DeactivateRoleAsync(Guid id, CancellationToken cancellationToken = default);
    }
}