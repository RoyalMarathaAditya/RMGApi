using HRMS.Api.Models;

namespace HRMS.Api.Repositories
{
    public interface IClientRepository
    {
        Task<IEnumerable<Client>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Client?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Client> CreateAsync(Client client, CancellationToken cancellationToken = default);
        Task<Client> UpdateAsync(Client client, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
