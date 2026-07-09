using HRMS.Api.Data;
using HRMS.Api.DTOs.UserDtos;
using HRMS.Api.Models;
using HRMS.Api.Services.Interfaces.UserManagement;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HRMS.Api.Services.UserManagement
{
    public class RoleManagementService : IRoleManagementService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<RoleManagementService> _logger;

        public RoleManagementService(AppDbContext dbContext, ILogger<RoleManagementService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<List<RoleDto>> GetRolesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.RoleMasters
                .AsNoTracking()
                .Where(r => !r.IsDeleted)
                .Select(r => new RoleDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    IsActive = r.IsActive,
                    UserCount = _dbContext.Users.Count(u => u.Role == r.Name && !u.IsDeleted),
                    CreatedOn = r.CreatedOn
                })
                .OrderBy(r => r.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<RoleDto?> GetRoleByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var role = await _dbContext.RoleMasters
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted, cancellationToken);

            if (role is null) return null;

            return new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                IsActive = role.IsActive,
                UserCount = await _dbContext.Users.CountAsync(u => u.Role == role.Name && !u.IsDeleted, cancellationToken),
                CreatedOn = role.CreatedOn
            };
        }

        public async Task<ApiResponse<RoleDto>> CreateRoleAsync(CreateRoleDto dto, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return ApiResponse<RoleDto>.Fail("Role name is required.");

            if (await _dbContext.RoleMasters.AnyAsync(r => r.Name == dto.Name && !r.IsDeleted, cancellationToken))
                return ApiResponse<RoleDto>.Fail("Role name already exists.");

            var role = new RoleMaster
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                IsActive = dto.IsActive,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "System"
            };

            _dbContext.RoleMasters.Add(role);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return ApiResponse<RoleDto>.Ok(new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                IsActive = role.IsActive,
                UserCount = 0,
                CreatedOn = role.CreatedOn
            }, "Role created successfully.");
        }

        public async Task<ApiResponse<RoleDto>> UpdateRoleAsync(Guid id, UpdateRoleDto dto, CancellationToken cancellationToken = default)
        {
            var role = await _dbContext.RoleMasters.FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted, cancellationToken);
            if (role is null)
                return ApiResponse<RoleDto>.Fail("Role not found.");

            if (!string.IsNullOrWhiteSpace(dto.Name) && dto.Name != role.Name)
            {
                if (await _dbContext.RoleMasters.AnyAsync(r => r.Name == dto.Name && r.Id != id && !r.IsDeleted, cancellationToken))
                    return ApiResponse<RoleDto>.Fail("Role name already exists.");
                role.Name = dto.Name;
            }

            if (dto.Description != null)
                role.Description = dto.Description;

            if (dto.IsActive.HasValue)
                role.IsActive = dto.IsActive.Value;

            role.ModifiedOn = DateTime.UtcNow;
            role.ModifiedBy = "System";

            await _dbContext.SaveChangesAsync(cancellationToken);

            return ApiResponse<RoleDto>.Ok(new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                IsActive = role.IsActive,
                UserCount = await _dbContext.Users.CountAsync(u => u.Role == role.Name && !u.IsDeleted, cancellationToken),
                CreatedOn = role.CreatedOn
            }, "Role updated successfully.");
        }

        public async Task<ApiResponse<bool>> DeleteRoleAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var role = await _dbContext.RoleMasters.FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted, cancellationToken);
            if (role is null)
                return ApiResponse<bool>.Fail("Role not found.");

            var userCount = await _dbContext.Users.CountAsync(u => u.Role == role.Name && !u.IsDeleted, cancellationToken);
            if (userCount > 0)
                return ApiResponse<bool>.Fail("Cannot delete role. It is assigned to users.");

            role.IsDeleted = true;
            role.ModifiedOn = DateTime.UtcNow;
            role.ModifiedBy = "System";

            await _dbContext.SaveChangesAsync(cancellationToken);
            return ApiResponse<bool>.Ok(true, "Role deleted successfully.");
        }

        public async Task<ApiResponse<bool>> ActivateRoleAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var role = await _dbContext.RoleMasters.FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted, cancellationToken);
            if (role is null)
                return ApiResponse<bool>.Fail("Role not found.");

            role.IsActive = true;
            role.ModifiedOn = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.Ok(true, "Role activated successfully.");
        }

        public async Task<ApiResponse<bool>> DeactivateRoleAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var role = await _dbContext.RoleMasters.FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted, cancellationToken);
            if (role is null)
                return ApiResponse<bool>.Fail("Role not found.");

            role.IsActive = false;
            role.ModifiedOn = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.Ok(true, "Role deactivated successfully.");
        }
    }
}