using HRMS.Api.Data;
using HRMS.Api.Models;
using HRMS.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Repositories
{
    public class ProjectStatusRepository : IProjectStatusRepository
    {
        private readonly AppDbContext _dbContext;

        public ProjectStatusRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<ProjectStatusMaster>> GetAllActiveAsync(CancellationToken ct = default)
        {
            return await _dbContext.ProjectStatusMasters
                .AsNoTracking()
                .Where(ps => ps.IsActive)
                .OrderBy(ps => ps.DisplayOrder)
                .ToListAsync(ct);
        }
    }
}
