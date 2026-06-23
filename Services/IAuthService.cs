using HRMS.Api.DTOs;

namespace HRMS.Api.Services
{
    public interface IAuthService
    {
        Task<LoginResponse?> AuthenticateAsync(LoginRequest request, CancellationToken cancellationToken = default);
        Task<LoginResponse?> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
        Task<UserDto?> GetCurrentUserAsync(int userId, CancellationToken cancellationToken = default);
        Task<bool> RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    }
}
