using HRMS.Api.Data;
using HRMS.Api.Models;
using HRMS.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Repositories
{
    public class DesignationRepository : IDesignationRepository
    {
        private readonly AppDbContext _dbContext;

        public DesignationRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<DesignationMaster>> GetAllAsync(CancellationToken ct = default)
        {
            return await _dbContext.DesignationMasters
                .AsNoTracking()
                .OrderBy(d => d.SortOrder)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<DesignationMaster>> GetActiveAsync(CancellationToken ct = default)
        {
            return await _dbContext.DesignationMasters
                .AsNoTracking()
                .Where(d => d.IsActive)
                .OrderBy(d => d.SortOrder)
                .ToListAsync(ct);
        }

        public async Task<DesignationMaster?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _dbContext.DesignationMasters
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id, ct);
        }

        public async Task<DesignationMaster> CreateAsync(DesignationMaster designation, CancellationToken ct = default)
        {
            _dbContext.DesignationMasters.Add(designation);
            await _dbContext.SaveChangesAsync(ct);
            return designation;
        }

        public async Task<DesignationMaster> UpdateAsync(DesignationMaster designation, CancellationToken ct = default)
        {
            _dbContext.DesignationMasters.Update(designation);
            await _dbContext.SaveChangesAsync(ct);
            return designation;
        }

        public async Task DeleteAsync(DesignationMaster designation, CancellationToken ct = default)
        {
            designation.IsDeleted = true;
            _dbContext.DesignationMasters.Update(designation);
            await _dbContext.SaveChangesAsync(ct);
        }

        public async Task<bool> ExistsAsync(string name, CancellationToken ct = default)
        {
            return await _dbContext.DesignationMasters
                .IgnoreQueryFilters()
                .AnyAsync(d => d.Name == name, ct);
        }

        public async Task<bool> CodeExistsAsync(string code, CancellationToken ct = default)
        {
            return await _dbContext.DesignationMasters
                .IgnoreQueryFilters()
                .AnyAsync(d => d.Code == code && d.Code != string.Empty, ct);
        }
    }
}