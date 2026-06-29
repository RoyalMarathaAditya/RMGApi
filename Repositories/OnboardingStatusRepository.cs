using HRMS.Api.Data;
using HRMS.Api.Models;
using HRMS.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Repositories
{
    public class OnboardingStatusRepository : IOnboardingStatusRepository
    {
        private readonly AppDbContext _dbContext;

        public OnboardingStatusRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<OnboardingStatusMaster>> GetAllActiveAsync(CancellationToken ct = default)
        {
            return await _dbContext.OnboardingStatusMasters
                .AsNoTracking()
                .Where(p => p.IsActive)
                .OrderBy(p => p.DisplayOrder)
                .ToListAsync(ct);
        }
    }
}
