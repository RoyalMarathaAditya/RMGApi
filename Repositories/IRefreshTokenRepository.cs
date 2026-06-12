using HRMS.Api.Models;

namespace HRMS.Api.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken token, CancellationToken cancellationToken = default);
        Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
        Task DeleteAsync(RefreshToken token, CancellationToken cancellationToken = default);
    }
}
