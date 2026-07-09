using HRMS.Api.DTOs.Common;
using HRMS.Api.DTOs.UserDtos;
using HRMS.Api.Models;

namespace HRMS.Api.Services.Interfaces.UserManagement
{
    public interface IUserManagementService
    {
        Task<PagedResponse<UserListDto>> GetUsersAsync(PaginationParams pagination, CancellationToken cancellationToken = default);
        Task<UserListDto?> GetUserByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<ApiResponse<UserListDto>> CreateUserAsync(CreateUserDto dto, string? createdBy, CancellationToken cancellationToken = default);
        Task<ApiResponse<UserListDto>> UpdateUserAsync(int id, UpdateUserDto dto, string? modifiedBy, CancellationToken cancellationToken = default);
        Task<ApiResponse<bool>> DeleteUserAsync(int id, CancellationToken cancellationToken = default);
        Task<ApiResponse<bool>> ResetPasswordAsync(ResetPasswordDto dto, CancellationToken cancellationToken = default);
        Task<ApiResponse<bool>> LockUserAsync(int id, string? lockedBy, CancellationToken cancellationToken = default);
        Task<ApiResponse<bool>> UnlockUserAsync(int id, CancellationToken cancellationToken = default);
        Task<ApiResponse<bool>> ActivateUserAsync(int id, string? modifiedBy, CancellationToken cancellationToken = default);
        Task<ApiResponse<bool>> DeactivateUserAsync(int id, string? modifiedBy, CancellationToken cancellationToken = default);
        Task<List<Employee>> GetAvailableEmployeesAsync(CancellationToken cancellationToken = default);
    }
}

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }

    public static ApiResponse<T> Ok(T data, string message = "") =>
        new() { Success = true, Data = data, Message = message };
    public static ApiResponse<T> Fail(string message) =>
        new() { Success = false, Message = message };
}