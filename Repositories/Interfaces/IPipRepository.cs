using HRMS.Api.Models;

namespace HRMS.Api.Repositories.Interfaces
{
    public interface IPipRepository
    {
        Task<IEnumerable<PIP>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<PIP?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<PIP> CreateAsync(PIP pip, CancellationToken cancellationToken = default);
        Task<PIP> UpdateAsync(PIP pip, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
