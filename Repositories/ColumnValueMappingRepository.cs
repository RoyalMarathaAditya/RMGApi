using HRMS.Api.Data;
using HRMS.Api.Models;
using HRMS.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Repositories
{
    public class ColumnValueMappingRepository : IColumnValueMappingRepository
    {
        private readonly AppDbContext _dbContext;

        public ColumnValueMappingRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<ColumnValueMapping>> GetAllAsync(CancellationToken ct = default)
        {
            return await _dbContext.Set<ColumnValueMapping>()
                .AsNoTracking()
                .OrderBy(m => m.TargetProperty)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<ColumnValueMapping>> GetActiveByTargetPropertyAsync(string targetProperty, CancellationToken ct = default)
        {
            return await _dbContext.Set<ColumnValueMapping>()
                .AsNoTracking()
                .Where(m => m.TargetProperty == targetProperty && m.IsActive)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<ColumnValueMapping>> GetAllActiveAsync(CancellationToken ct = default)
        {
            return await _dbContext.Set<ColumnValueMapping>()
                .AsNoTracking()
                .Where(m => m.IsActive)
                .ToListAsync(ct);
        }

        public async Task<ColumnValueMapping?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _dbContext.Set<ColumnValueMapping>()
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id, ct);
        }

        public async Task<ColumnValueMapping> CreateAsync(ColumnValueMapping mapping, CancellationToken ct = default)
        {
            _dbContext.Set<ColumnValueMapping>().Add(mapping);
            await _dbContext.SaveChangesAsync(ct);
            return mapping;
        }

        public async Task<ColumnValueMapping> UpdateAsync(ColumnValueMapping mapping, CancellationToken ct = default)
        {
            _dbContext.Set<ColumnValueMapping>().Update(mapping);
            await _dbContext.SaveChangesAsync(ct);
            return mapping;
        }

        public async Task DeleteAsync(ColumnValueMapping mapping, CancellationToken ct = default)
        {
            _dbContext.Set<ColumnValueMapping>().Remove(mapping);
            await _dbContext.SaveChangesAsync(ct);
        }
    }
}
