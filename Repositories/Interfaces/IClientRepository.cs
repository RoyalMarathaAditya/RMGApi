using HRMS.Api.Models;

namespace HRMS.Api.Repositories.Interfaces
{
    public interface IClientRepository
    {
        Task<IEnumerable<Client>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Client?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Client> CreateAsync(Client client, CancellationToken cancellationToken = default);
        Task<Client> UpdateAsync(Client client, string rowVersion, CancellationToken cancellationToken = default);
        Task SoftDeleteAsync(Client client, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(string name, CancellationToken cancellationToken = default);
    }
}
