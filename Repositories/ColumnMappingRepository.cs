using HRMS.Api.Data;
using HRMS.Api.Models;
using HRMS.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Repositories
{
    public class ColumnMappingRepository : IColumnMappingRepository
    {
        private readonly AppDbContext _dbContext;

        public ColumnMappingRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<ColumnMapping>> GetAllAsync(CancellationToken ct = default)
        {
            return await _dbContext.Set<ColumnMapping>()
                .AsNoTracking()
                .OrderBy(m => m.DisplayOrder)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<ColumnMapping>> GetAllActiveAsync(CancellationToken ct = default)
        {
            return await _dbContext.Set<ColumnMapping>()
                .AsNoTracking()
                .Where(m => m.IsActive)
                .OrderBy(m => m.DisplayOrder)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<ColumnMapping>> GetByEntityTypeAsync(string entityType, bool activeOnly = true, CancellationToken ct = default)
        {
            var query = _dbContext.Set<ColumnMapping>()
                .AsNoTracking()
                .Where(m => m.EntityType == entityType);

            if (activeOnly)
                query = query.Where(m => m.IsActive);

            return await query
                .OrderBy(m => m.DisplayOrder)
                .ToListAsync(ct);
        }

        public async Task<ColumnMapping?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _dbContext.Set<ColumnMapping>()
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id, ct);
        }

        public async Task<bool> ExistsByEntityTypeAndSourceColumnAsync(string entityType, string sourceColumn, Guid? excludeId = null, CancellationToken ct = default)
        {
            return await _dbContext.Set<ColumnMapping>()
                .AnyAsync(m =>
                    m.IsActive &&
                    m.EntityType == entityType &&
                    EF.Functions.Like(m.SourceColumn, sourceColumn) &&
                    (!excludeId.HasValue || m.Id != excludeId.Value), ct);
        }

        public async Task<bool> ExistsByEntityTypeAndTargetDisplayNameAsync(string entityType, string targetDisplayName, Guid? excludeId = null, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(targetDisplayName))
                return false;

            return await _dbContext.Set<ColumnMapping>()
                .AnyAsync(m =>
                    m.IsActive &&
                    m.EntityType == entityType &&
                    EF.Functions.Like(m.TargetDisplayName, targetDisplayName) &&
                    (!excludeId.HasValue || m.Id != excludeId.Value), ct);
        }

        public async Task<ColumnMapping> CreateAsync(ColumnMapping mapping, CancellationToken ct = default)
        {
            _dbContext.Set<ColumnMapping>().Add(mapping);
            await _dbContext.SaveChangesAsync(ct);
            return mapping;
        }

        public async Task<ColumnMapping> UpdateAsync(ColumnMapping mapping, CancellationToken ct = default)
        {
            _dbContext.Set<ColumnMapping>().Update(mapping);
            await _dbContext.SaveChangesAsync(ct);
            return mapping;
        }

        public async Task DeleteAsync(ColumnMapping mapping, CancellationToken ct = default)
        {
            _dbContext.Set<ColumnMapping>().Remove(mapping);
            await _dbContext.SaveChangesAsync(ct);
        }
    }
}
