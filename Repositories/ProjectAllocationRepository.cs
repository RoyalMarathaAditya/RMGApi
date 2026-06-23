using HRMS.Api.Data;
using HRMS.Api.Models;
using HRMS.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Repositories
{
    public class ProjectAllocationRepository : IProjectAllocationRepository
    {
        private readonly AppDbContext _dbContext;

        public ProjectAllocationRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<ProjectAllocation>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProjectAllocations
                .AsNoTracking()
                .Include(pa => pa.Employee)
                .Include(pa => pa.Project)
                .Include(pa => pa.EmployeeProjectStatus)
                .Include(pa => pa.AllocationStatus)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<ProjectAllocation>> GetByEmployeeIdAsync(int employeeId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProjectAllocations
                .AsNoTracking()
                .Include(pa => pa.Project)
                .Include(pa => pa.EmployeeProjectStatus)
                .Include(pa => pa.AllocationStatus)
                .Where(pa => pa.EmployeeId == employeeId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<ProjectAllocation>> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProjectAllocations
                .AsNoTracking()
                .Include(pa => pa.Employee)
                .Include(pa => pa.EmployeeProjectStatus)
                .Include(pa => pa.AllocationStatus)
                .Where(pa => pa.ProjectId == projectId)
                .ToListAsync(cancellationToken);
        }

        public async Task<ProjectAllocation?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProjectAllocations
                .Include(pa => pa.Employee)
                .Include(pa => pa.Project)
                .Include(pa => pa.EmployeeProjectStatus)
                .Include(pa => pa.AllocationStatus)
                .FirstOrDefaultAsync(pa => pa.Id == id, cancellationToken);
        }

        public async Task<ProjectAllocation> CreateAsync(ProjectAllocation allocation, CancellationToken cancellationToken = default)
        {
            _dbContext.ProjectAllocations.Add(allocation);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return allocation;
        }

        public async Task<ProjectAllocation> UpdateAsync(ProjectAllocation allocation, CancellationToken cancellationToken = default)
        {
            _dbContext.ProjectAllocations.Update(allocation);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return allocation;
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var allocation = await _dbContext.ProjectAllocations.FindAsync(new object[] { id }, cancellationToken);
            if (allocation is null) return;
            allocation.IsDeleted = true;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
