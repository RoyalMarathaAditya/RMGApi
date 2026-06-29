using HRMS.Api.Data;
using HRMS.Api.Models;
using HRMS.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Repositories
{
    public class BillingBucketRepository : IBillingBucketRepository
    {
        private readonly AppDbContext _dbContext;

        public BillingBucketRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<BillingBucketMaster>> GetAllActiveAsync(CancellationToken ct = default)
        {
            return await _dbContext.BillingBucketMasters
                .AsNoTracking()
                .Where(p => p.IsActive)
                .OrderBy(p => p.DisplayOrder)
                .ToListAsync(ct);
        }
    }
}
