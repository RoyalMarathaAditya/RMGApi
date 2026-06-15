using HRMS.Api.Data;
using HRMS.Api.Models;
using HRMS.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Repositories
{
    public class ProjectDetailsRepository : IProjectDetailsRepository
    {
        private readonly AppDbContext _dbContext;

        public ProjectDetailsRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<ProjectDetails>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProjectDetails
                .Include(p => p.Client)
                .Include(p => p.Location)
                .Include(p => p.ProjectSkills).ThenInclude(ps => ps.Skill)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<ProjectDetails?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProjectDetails
                .Include(p => p.Client)
                .Include(p => p.Location)
                .Include(p => p.ProjectSkills).ThenInclude(ps => ps.Skill)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<ProjectDetails> CreateAsync(ProjectDetails project, CancellationToken cancellationToken = default)
        {
            _dbContext.ProjectDetails.Add(project);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return project;
        }

        public async Task<ProjectDetails> UpdateAsync(ProjectDetails project, CancellationToken cancellationToken = default)
        {
            _dbContext.ProjectDetails.Update(project);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return project;
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var p = await GetByIdAsync(id, cancellationToken);
            if (p is null) return;
            _dbContext.ProjectDetails.Remove(p);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
