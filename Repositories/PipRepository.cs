using HRMS.Api.Data;
using HRMS.Api.Models;
using HRMS.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Repositories
{
    public class PipRepository : IPipRepository
    {
        private readonly AppDbContext _dbContext;

        public PipRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<PIP>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.PIPs
                .AsNoTracking()
                .Include(p => p.Employee)
                .ToListAsync(cancellationToken);
        }

        public async Task<PIP?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.PIPs
                .Include(p => p.Employee)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<PIP> CreateAsync(PIP pip, CancellationToken cancellationToken = default)
        {
            _dbContext.PIPs.Add(pip);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return pip;
        }

        public async Task<PIP> UpdateAsync(PIP pip, CancellationToken cancellationToken = default)
        {
            _dbContext.PIPs.Update(pip);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return pip;
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var pip = await _dbContext.PIPs.FindAsync(new object[] { id }, cancellationToken);
            if (pip is null) return;
            pip.IsDeleted = true;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
