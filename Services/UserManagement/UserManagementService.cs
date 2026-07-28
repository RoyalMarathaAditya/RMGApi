using System.Diagnostics;
using HRMS.Api.DTOs.Common;
using HRMS.Api.DTOs.UserDtos;
using HRMS.Api.Models;
using HRMS.Api.Repositories.Interfaces;
using HRMS.Api.Services.Interfaces;
using HRMS.Api.Services.Interfaces.UserManagement;

namespace HRMS.Api.Services.UserManagement
{
    public class UserManagementService : IUserManagementService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICacheService _cache;
        private readonly ILogger<UserManagementService> _logger;
        private const string DefaultPassword = "NV@12345#";
        private const string UsersCachePrefix = "users";
        private const string RolesCacheKey = "roles";
        private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(5);

        public UserManagementService(IUserRepository userRepository, ICacheService cache, ILogger<UserManagementService> logger)
        {
            _userRepository = userRepository;
            _cache = cache;
            _logger = logger;
        }

        public async Task<PagedResponse<UserListDto>> GetUsersAsync(PaginationParams pagination, CancellationToken cancellationToken = default)
        {
            var sw = Stopwatch.StartNew();

            var cacheKey = $"{UsersCachePrefix}:page={pagination.PageNumber}&size={pagination.PageSize}&search={pagination.SearchTerm ?? ""}&sort={pagination.SortBy ?? "name"}&desc={pagination.SortDescending}&role={pagination.RoleIdFilter ?? ""}&status={pagination.StatusFilter ?? ""}";

            var result = await _cache.GetOrCreateAsync(cacheKey, async () =>
                (PagedResponse<UserListDto>?)await _userRepository.GetPagedAsync(pagination, cancellationToken), new CacheEntryOptions
                {
                    DistributedExpiration = CacheTtl,
                    MemoryExpiration = TimeSpan.FromMinutes(1)
                });

            sw.Stop();
            if (sw.ElapsedMilliseconds > 150)
                _logger.LogWarning("GetUsersAsync took {ElapsedMs}ms for page {Page}", sw.ElapsedMilliseconds, pagination.PageNumber);

            return result ?? new PagedResponse<UserListDto>();
        }

        public async Task<UserListDto?> GetUserByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _cache.GetOrCreateAsync($"user:{id}", () =>
                _userRepository.GetByIdProjectedAsync(id, cancellationToken), new CacheEntryOptions
                {
                    DistributedExpiration = TimeSpan.FromMinutes(10),
                    MemoryExpiration = TimeSpan.FromMinutes(2)
                });
        }

        public async Task<ApiResponse<UserListDto>> CreateUserAsync(CreateUserDto dto, string? createdBy, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(dto.UserName))
                return ApiResponse<UserListDto>.Fail("Username is required.");

            if (string.IsNullOrWhiteSpace(dto.Email))
                return ApiResponse<UserListDto>.Fail("Email is required.");

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
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(DefaultPassword),
                RoleId = dto.RoleId,
                EmployeeId = dto.EmployeeId,
                IsActive = dto.IsActive,
                IsFirstLogin = true,
                IsDefaultPassword = true,
                PasswordResetRequired = true,
                CreatedBy = createdBy,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var created = await _userRepository.CreateAsync(user, cancellationToken);

            _logger.LogInformation("User {UserId} created with default password by {CreatedBy}", created.Id, createdBy);

            await InvalidateUsersCacheAsync();

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
                CreatedAt = created.CreatedAt,
                IsFirstLogin = created.IsFirstLogin,
                IsDefaultPassword = created.IsDefaultPassword,
                PasswordResetRequired = created.PasswordResetRequired
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

            user.ModifiedBy = modifiedBy;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user, cancellationToken);

            await InvalidateUsersCacheAsync();

            return ApiResponse<UserListDto>.Ok(MapToDto(user), "User updated successfully.");
        }

        public async Task<ApiResponse<bool>> DeleteUserAsync(int id, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByIdAsync(id, cancellationToken);
            if (user is null)
                return ApiResponse<bool>.Fail("User not found.");

            await _userRepository.DeleteAsync(user, cancellationToken);

            await InvalidateUsersCacheAsync();

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

            await InvalidateUsersCacheAsync();

            return ApiResponse<bool>.Ok(true, "Password reset successfully.");
        }

        public async Task<ApiResponse<bool>> ResetPasswordToDefaultAsync(int id, string? resetBy, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByIdAsync(id, cancellationToken);
            if (user is null)
                return ApiResponse<bool>.Fail("User not found.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(DefaultPassword);
            user.IsFirstLogin = true;
            user.PasswordResetRequired = true;
            user.IsDefaultPassword = true;
            user.PasswordChangedOn = null;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user, cancellationToken);

            _logger.LogInformation("Password reset to default for user {UserId} by {ResetBy}", id, resetBy);

            await InvalidateUsersCacheAsync();

            return ApiResponse<bool>.Ok(true, $"Password has been reset to {DefaultPassword}");
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

            await InvalidateUsersCacheAsync();

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

            await InvalidateUsersCacheAsync();

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

            await InvalidateUsersCacheAsync();

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

            await InvalidateUsersCacheAsync();

            return ApiResponse<bool>.Ok(true, "User deactivated successfully.");
        }

        public async Task<List<Employee>> GetAvailableEmployeesAsync(CancellationToken cancellationToken = default)
        {
            var cached = await _cache.GetOrCreateAsync("employees:without-user", async () =>
                (List<Employee>?)await _userRepository.GetEmployeesWithoutUserAsync(cancellationToken), new CacheEntryOptions
                {
                    DistributedExpiration = TimeSpan.FromMinutes(10),
                    MemoryExpiration = TimeSpan.FromMinutes(5)
                });
            return cached ?? new List<Employee>();
        }

        public async Task InvalidateUsersCacheAsync()
        {
            await _cache.RemoveByPrefixAsync(UsersCachePrefix);
            await _cache.RemoveAsync($"user:*");
            await _cache.RemoveAsync("employees:without-user");
        }

        private static UserListDto MapToDto(User user)
        {
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
                LockedBy = user.LockedBy,
                IsFirstLogin = user.IsFirstLogin,
                IsDefaultPassword = user.IsDefaultPassword,
                PasswordChangedOn = user.PasswordChangedOn,
                PasswordResetRequired = user.PasswordResetRequired
            };
        }
    }
}
