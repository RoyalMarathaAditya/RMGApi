using HRMS.Api.Data;
using HRMS.Api.Models;
using HRMS.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Repositories
{
    public class ProbableNextAssignmentRepository : IProbableNextAssignmentRepository
    {
        private readonly AppDbContext _dbContext;

        public ProbableNextAssignmentRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<ProbableNextAssignmentMaster>> GetAllActiveAsync(CancellationToken ct = default)
        {
            return await _dbContext.ProbableNextAssignmentMasters
                .AsNoTracking()
                .Where(p => p.IsActive)
                .OrderBy(p => p.DisplayOrder)
                .ToListAsync(ct);
        }
    }
}
