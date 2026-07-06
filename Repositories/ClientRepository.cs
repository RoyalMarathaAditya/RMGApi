using HRMS.Api.Data;
using HRMS.Api.Models;
using HRMS.Api.Repositories.Interfaces;
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

        public async Task<IEnumerable<Client>> GetAllAsync(CancellationToken ct = default)
        {
            return await _dbContext.Clients
                .AsNoTracking()
                .Include(c => c.ClientStatus)
                .OrderBy(c => c.Name)
                .ToListAsync(ct);
        }

        public async Task<Client?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _dbContext.Clients
                .AsNoTracking()
                .Include(c => c.ClientStatus)
                .FirstOrDefaultAsync(c => c.Id == id, ct);
        }

        public async Task<Client> CreateAsync(Client client, CancellationToken ct = default)
        {
            _dbContext.Clients.Add(client);
            await _dbContext.SaveChangesAsync(ct);
            return await GetByIdAsync(client.Id, ct) ?? client;
        }

        public async Task<Client> UpdateAsync(Client client, string rowVersion, CancellationToken ct = default)
        {
            var existing = await _dbContext.Clients
                .Include(c => c.ClientStatus)
                .FirstOrDefaultAsync(c => c.Id == client.Id, ct);

            if (existing is null)
                throw new KeyNotFoundException("Client not found");

            var existingRowVersion = Convert.ToBase64String(existing.RowVersion);
            if (existingRowVersion != rowVersion)
                throw new DbUpdateConcurrencyException("RowVersion mismatch");

            existing.Name = client.Name;
            existing.ContractStartDate = client.ContractStartDate;
            existing.ContractEndDate = client.ContractEndDate;
            existing.StatusId = client.StatusId;
            existing.Location = client.Location;
            existing.ModifiedBy = client.ModifiedBy;
            existing.ModifiedOn = client.ModifiedOn;

            await _dbContext.SaveChangesAsync(ct);

            existing.ClientStatus = await _dbContext.StatusMasters
                .AsNoTracking()
                .FirstAsync(s => s.Id == existing.StatusId, ct);

            return existing;
        }

        public async Task SoftDeleteAsync(Client client, CancellationToken ct = default)
        {
            var existing = await _dbContext.Clients
                .FirstOrDefaultAsync(c => c.Id == client.Id, ct);

            if (existing is null) return;

            existing.IsDeleted = true;
            existing.ModifiedBy = client.ModifiedBy;
            existing.ModifiedOn = client.ModifiedOn;

            await _dbContext.SaveChangesAsync(ct);
        }

        public async Task<bool> ExistsAsync(string name, CancellationToken ct = default)
        {
            return await _dbContext.Clients
                .IgnoreQueryFilters()
                .AnyAsync(c => c.Name == name && !c.IsDeleted, ct);
        }
    }
}
