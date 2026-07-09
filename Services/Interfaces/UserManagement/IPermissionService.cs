using HRMS.Api.DTOs.UserDtos;

namespace HRMS.Api.Services.Interfaces.UserManagement
{
    public interface IPermissionService
    {
        Task<List<PermissionDto>> GetAllPermissionsAsync(CancellationToken cancellationToken = default);
        Task<List<PermissionDto>> GetPermissionsByRoleAsync(string roleName, CancellationToken cancellationToken = default);
        Task<ApiResponse<bool>> SaveRolePermissionsAsync(RolePermissionDto dto, CancellationToken cancellationToken = default);
    }
}