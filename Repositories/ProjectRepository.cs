using HRMS.Api.Data;
using HRMS.Api.Models;
using HRMS.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly AppDbContext _dbContext;

        public ProjectRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Project>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Projects
                .Include(p => p.Client)
                .Include(p => p.CSMRevenueType)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Project>> GetActiveProjectsAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Projects
                .Include(p => p.Client)
                .Include(p => p.CSMRevenueType)
                .Where(p => p.IsActive)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<Project?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Projects
                .Include(p => p.Client)
                .Include(p => p.CSMRevenueType)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<Project> CreateAsync(Project project, CancellationToken cancellationToken = default)
        {
            await _dbContext.Projects.AddAsync(project, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return project;
        }

        public async Task<Project> UpdateAsync(Project project, CancellationToken cancellationToken = default)
        {
            _dbContext.Projects.Update(project);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return project;
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var project = await _dbContext.Projects.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (project == null) return;
            project.IsDeleted = true;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
