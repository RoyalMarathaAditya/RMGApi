using HRMS.Api.Data;
using HRMS.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly AppDbContext _dbContext;

        public ClientRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Client>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Clients.AsNoTracking().ToListAsync(cancellationToken);
        }

        public async Task<Client?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Clients.FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<Client> CreateAsync(Client client, CancellationToken cancellationToken = default)
        {
            _dbContext.Clients.Add(client);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return client;
        }

        public async Task<Client> UpdateAsync(Client client, CancellationToken cancellationToken = default)
        {
            _dbContext.Clients.Update(client);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return client;
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var client = await GetByIdAsync(id, cancellationToken);
            if (client is null) return;
            _dbContext.Clients.Remove(client);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
