using HRMS.Api.Models;

namespace HRMS.Api.Repositories
{
    public interface IAuthRepository
    {
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    }
}
