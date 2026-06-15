using HRMS.Api.Models;

namespace HRMS.Api.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    }
}
