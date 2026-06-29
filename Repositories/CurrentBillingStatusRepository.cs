using HRMS.Api.Data;
using HRMS.Api.Models;
using HRMS.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Repositories
{
    public class CurrentBillingStatusRepository : ICurrentBillingStatusRepository
    {
        private readonly AppDbContext _dbContext;

        public CurrentBillingStatusRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<CurrentBillingStatusMaster>> GetAllActiveAsync(CancellationToken ct = default)
        {
            return await _dbContext.CurrentBillingStatusMasters
                .AsNoTracking()
                .Where(p => p.IsActive)
                .OrderBy(p => p.DisplayOrder)
                .ToListAsync(ct);
        }
    }
}
