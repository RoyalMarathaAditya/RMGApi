using HRMS.Api.Data;
using HRMS.Api.Models;
using HRMS.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Repositories
{
    public class AgeingBucketRepository : IAgeingBucketRepository
    {
        private readonly AppDbContext _dbContext;

        public AgeingBucketRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<AgeingBucketMaster>> GetAllActiveAsync(CancellationToken ct = default)
        {
            return await _dbContext.AgeingBucketMasters
                .AsNoTracking()
                .Where(p => p.IsActive)
                .OrderBy(p => p.DisplayOrder)
                .ToListAsync(ct);
        }
    }
}
