using HRMS.Api.DTOs.Common;
using HRMS.Api.DTOs.UserDtos;
using HRMS.Api.Models;
using HRMS.Api.Repositories.Interfaces;
using HRMS.Api.Services.Interfaces.UserManagement;
using Microsoft.Extensions.Logging;

namespace HRMS.Api.Services.UserManagement
{
    public class UserManagementService : IUserManagementService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserManagementService> _logger;

        public UserManagementService(IUserRepository userRepository, ILogger<UserManagementService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<PagedResponse<UserListDto>> GetUsersAsync(PaginationParams pagination, CancellationToken cancellationToken = default)
        {
            return await _userRepository.GetPagedAsync(pagination, cancellationToken);
        }

        public async Task<UserListDto?> GetUserByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByIdAsync(id, cancellationToken);
            if (user is null) return null;

            return new UserListDto
            {
                Id = user.Id,
                Name = user.Name,
                UserName = user.UserName,
                Email = user.Email,
                Phone = user.Phone,
                RoleId = user.RoleId,
                RoleName = user.Role?.Name ?? string.Empty,
                EmployeeId = user.EmployeeId,
                EmployeeCode = user.Employee?.EmployeeCode,
                EmployeeName = user.Employee?.FullName,
                Designation = user.Employee?.Designation?.Name,
                Practice = user.Employee?.Practice?.Name,
                Department = user.Employee?.DepartmentType?.Name,
                IsActive = user.IsActive,
                IsLocked = user.IsLocked,
                LastLoginDate = user.LastLoginDate,
                CreatedAt = user.CreatedAt,
                CreatedBy = user.CreatedBy,
                ModifiedBy = user.ModifiedBy,
                ModifiedOn = user.ModifiedOn,
                FailedLoginCount = user.FailedLoginCount,
                LockedDate = user.LockedDate,
                LockedBy = user.LockedBy
            };
        }

        public async Task<ApiResponse<UserListDto>> CreateUserAsync(CreateUserDto dto, string? createdBy, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(dto.UserName))
                return ApiResponse<UserListDto>.Fail("Username is required.");

            if (string.IsNullOrWhiteSpace(dto.Email))
                return ApiResponse<UserListDto>.Fail("Email is required.");

            if (string.IsNullOrWhiteSpace(dto.Password))
                return ApiResponse<UserListDto>.Fail("Password is required.");

            if (dto.Password != dto.ConfirmPassword)
                return ApiResponse<UserListDto>.Fail("Passwords do not match.");

            if (!await _userRepository.IsEmailUniqueAsync(dto.Email, null, cancellationToken))
                return ApiResponse<UserListDto>.Fail("Email already exists.");

            if (!await _userRepository.IsUserNameUniqueAsync(dto.UserName, null, cancellationToken))
                return ApiResponse<UserListDto>.Fail("Username already exists.");



            if (dto.RoleId == Guid.Empty)
                return ApiResponse<UserListDto>.Fail("Role is required.");

            if (dto.EmployeeId.HasValue)
            {
                var existingUserForEmployee = await _userRepository.GetByEmployeeIdAsync(dto.EmployeeId.Value, cancellationToken);
                if (existingUserForEmployee != null)
                    return ApiResponse<UserListDto>.Fail("A user account already exists for the selected employee.");
            }

            var user = new User
            {
                Name = dto.Name,
                UserName = dto.UserName,
                Email = dto.Email,
                Phone = dto.Phone,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                RoleId = dto.RoleId,
                EmployeeId = dto.EmployeeId,
                IsActive = dto.IsActive,
                CreatedBy = createdBy,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var created = await _userRepository.CreateAsync(user, cancellationToken);

            return ApiResponse<UserListDto>.Ok(new UserListDto
            {
                Id = created.Id,
                Name = created.Name,
                UserName = created.UserName,
                Email = created.Email,
                Phone = created.Phone,
                RoleId = created.RoleId,
                RoleName = created.Role?.Name ?? string.Empty,
                EmployeeId = created.EmployeeId,
                IsActive = created.IsActive,
                CreatedAt = created.CreatedAt
            }, "User created successfully.");
        }

        public async Task<ApiResponse<UserListDto>> UpdateUserAsync(int id, UpdateUserDto dto, string? modifiedBy, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByIdAsync(id, cancellationToken);
            if (user is null)
                return ApiResponse<UserListDto>.Fail("User not found.");

            if (!string.IsNullOrWhiteSpace(dto.Email) && dto.Email != user.Email)
            {
                if (!await _userRepository.IsEmailUniqueAsync(dto.Email, id, cancellationToken))
                    return ApiResponse<UserListDto>.Fail("Email already exists.");
                user.Email = dto.Email;
            }

            if (dto.Phone is not null && dto.Phone != user.Phone)
                user.Phone = dto.Phone;

            if (dto.RoleId.HasValue && dto.RoleId.Value != Guid.Empty)
                user.RoleId = dto.RoleId.Value;

            if (dto.IsActive.HasValue)
                user.IsActive = dto.IsActive.Value;

            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                if (dto.Password != dto.ConfirmPassword)
                    return ApiResponse<UserListDto>.Fail("Passwords do not match.");
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            }

            user.ModifiedBy = modifiedBy;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user, cancellationToken);

            return ApiResponse<UserListDto>.Ok(new UserListDto
            {
                Id = user.Id,
                Name = user.Name,
                UserName = user.UserName,
                Email = user.Email,
                Phone = user.Phone,
                RoleId = user.RoleId,
                RoleName = user.Role?.Name ?? string.Empty,
                EmployeeId = user.EmployeeId,
                EmployeeCode = user.Employee?.EmployeeCode,
                EmployeeName = user.Employee?.FullName,
                Designation = user.Employee?.Designation?.Name,
                Practice = user.Employee?.Practice?.Name,
                Department = user.Employee?.DepartmentType?.Name,
                IsActive = user.IsActive,
                IsLocked = user.IsLocked,
                CreatedAt = user.CreatedAt,
                CreatedBy = user.CreatedBy,
                ModifiedBy = user.ModifiedBy,
                ModifiedOn = user.ModifiedOn,
                LastLoginDate = user.LastLoginDate,
                FailedLoginCount = user.FailedLoginCount,
                LockedDate = user.LockedDate,
                LockedBy = user.LockedBy
            }, "User updated successfully.");
        }

        public async Task<ApiResponse<bool>> DeleteUserAsync(int id, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByIdAsync(id, cancellationToken);
            if (user is null)
                return ApiResponse<bool>.Fail("User not found.");

            await _userRepository.DeleteAsync(user, cancellationToken);
            return ApiResponse<bool>.Ok(true, "User deleted successfully.");
        }

        public async Task<ApiResponse<bool>> ResetPasswordAsync(ResetPasswordDto dto, CancellationToken cancellationToken = default)
        {
            if (dto.NewPassword != dto.ConfirmPassword)
                return ApiResponse<bool>.Fail("Passwords do not match.");

            if (dto.NewPassword.Length < 8 || !dto.NewPassword.Any(char.IsUpper) ||
                !dto.NewPassword.Any(char.IsLower) || !dto.NewPassword.Any(char.IsDigit) ||
                !dto.NewPassword.Any(c => !char.IsLetterOrDigit(c)))
                return ApiResponse<bool>.Fail("Password must be at least 8 characters with uppercase, lowercase, number, and special character.");

            var user = await _userRepository.GetByIdAsync(dto.UserId, cancellationToken);
            if (user is null)
                return ApiResponse<bool>.Fail("User not found.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user, cancellationToken);
            return ApiResponse<bool>.Ok(true, "Password reset successfully.");
        }

        public async Task<ApiResponse<bool>> LockUserAsync(int id, string? lockedBy, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByIdAsync(id, cancellationToken);
            if (user is null)
                return ApiResponse<bool>.Fail("User not found.");

            user.IsLocked = true;
            user.LockedDate = DateTime.UtcNow;
            user.LockedBy = lockedBy;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user, cancellationToken);
            return ApiResponse<bool>.Ok(true, "User locked successfully.");
        }

        public async Task<ApiResponse<bool>> UnlockUserAsync(int id, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByIdAsync(id, cancellationToken);
            if (user is null)
                return ApiResponse<bool>.Fail("User not found.");

            user.IsLocked = false;
            user.FailedLoginCount = 0;
            user.LockedDate = null;
            user.LockedBy = null;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user, cancellationToken);
            return ApiResponse<bool>.Ok(true, "User unlocked successfully.");
        }

        public async Task<ApiResponse<bool>> ActivateUserAsync(int id, string? modifiedBy, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByIdAsync(id, cancellationToken);
            if (user is null)
                return ApiResponse<bool>.Fail("User not found.");

            user.IsActive = true;
            user.ModifiedBy = modifiedBy;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user, cancellationToken);
            return ApiResponse<bool>.Ok(true, "User activated successfully.");
        }

        public async Task<ApiResponse<bool>> DeactivateUserAsync(int id, string? modifiedBy, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByIdAsync(id, cancellationToken);
            if (user is null)
                return ApiResponse<bool>.Fail("User not found.");

            user.IsActive = false;
            user.ModifiedBy = modifiedBy;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user, cancellationToken);
            return ApiResponse<bool>.Ok(true, "User deactivated successfully.");
        }

        public async Task<List<Employee>> GetAvailableEmployeesAsync(CancellationToken cancellationToken = default)
        {
            return await _userRepository.GetEmployeesWithoutUserAsync(cancellationToken);
        }
    }
}