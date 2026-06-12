using HRMS.Api.DTOs;

namespace HRMS.Api.Services
{
    public interface IAuthService
    {
        Task<LoginResponse?> AuthenticateAsync(LoginRequest request, CancellationToken cancellationToken = default);
    }
}
