using HRMS.Api.Data;
using HRMS.Api.DTOs.UserDtos;
using HRMS.Api.Models.Permissions;
using HRMS.Api.Services.Interfaces.UserManagement;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HRMS.Api.Services.UserManagement
{
    public class PermissionService : IPermissionService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<PermissionService> _logger;

        public PermissionService(AppDbContext dbContext, ILogger<PermissionService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<List<PermissionDto>> GetAllPermissionsAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Permissions
                .AsNoTracking()
                .Where(p => p.IsActive)
                .OrderBy(p => p.Category)
                .ThenBy(p => p.Name)
                .Select(p => new PermissionDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Category = p.Category,
                    IsActive = p.IsActive
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<List<PermissionDto>> GetPermissionsByRoleAsync(string roleName, CancellationToken cancellationToken = default)
        {
            var permissionIds = await _dbContext.RolePermissions
                .Where(rp => rp.RoleName == roleName)
                .Select(rp => rp.PermissionId)
                .ToListAsync(cancellationToken);

            return await _dbContext.Permissions
                .AsNoTracking()
                .Where(p => p.IsActive)
                .OrderBy(p => p.Category)
                .ThenBy(p => p.Name)
                .Select(p => new PermissionDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Category = p.Category,
                    HasPermission = permissionIds.Contains(p.Id)
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<ApiResponse<bool>> SaveRolePermissionsAsync(RolePermissionDto dto, CancellationToken cancellationToken = default)
        {
            var existing = await _dbContext.RolePermissions
                .Where(rp => rp.RoleName == dto.RoleName)
                .ToListAsync(cancellationToken);

            _dbContext.RolePermissions.RemoveRange(existing);

            foreach (var permId in dto.PermissionIds)
            {
                _dbContext.RolePermissions.Add(new RolePermission
                {
                    RoleName = dto.RoleName,
                    PermissionId = permId
                });
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            return ApiResponse<bool>.Ok(true, "Role permissions updated successfully.");
        }
    }
}