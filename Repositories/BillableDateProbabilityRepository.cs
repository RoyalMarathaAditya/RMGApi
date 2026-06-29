using HRMS.Api.Data;
using HRMS.Api.Models;
using HRMS.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Repositories
{
    public class BillableDateProbabilityRepository : IBillableDateProbabilityRepository
    {
        private readonly AppDbContext _dbContext;

        public BillableDateProbabilityRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<BillableDateProbabilityMaster>> GetAllActiveAsync(CancellationToken ct = default)
        {
            return await _dbContext.BillableDateProbabilityMasters
                .AsNoTracking()
                .Where(p => p.IsActive)
                .OrderBy(p => p.DisplayOrder)
                .ToListAsync(ct);
        }
    }
}
