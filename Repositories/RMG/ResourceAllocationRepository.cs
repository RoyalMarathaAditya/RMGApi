using HRMS.Api.Data;
using HRMS.Api.Models.RMG;
using HRMS.Api.Repositories.Interfaces.RMG;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Repositories.RMG
{
    public class ResourceAllocationRepository : IResourceAllocationRepository
    {
        private readonly AppDbContext _dbContext;

        public ResourceAllocationRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<ResourceAllocation>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.ResourceAllocations
                .AsNoTracking()
                .Include(ra => ra.Employee)
                .Include(ra => ra.Project)
                .ToListAsync(cancellationToken);
        }

        public async Task<ResourceAllocation?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ResourceAllocations
                .Include(ra => ra.Employee)
                .Include(ra => ra.Project)
                .FirstOrDefaultAsync(ra => ra.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<ResourceAllocation>> GetByEmployeeIdAsync(int employeeId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ResourceAllocations
                .AsNoTracking()
                .Include(ra => ra.Project)
                .Where(ra => ra.EmployeeId == employeeId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<ResourceAllocation>> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ResourceAllocations
                .AsNoTracking()
                .Include(ra => ra.Employee)
                .Where(ra => ra.ProjectId == projectId)
                .ToListAsync(cancellationToken);
        }

        public async Task<ResourceAllocation> CreateAsync(ResourceAllocation allocation, CancellationToken cancellationToken = default)
        {
            _dbContext.ResourceAllocations.Add(allocation);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return allocation;
        }

        public async Task<ResourceAllocation> UpdateAsync(ResourceAllocation allocation, CancellationToken cancellationToken = default)
        {
            _dbContext.ResourceAllocations.Update(allocation);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return allocation;
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var allocation = await _dbContext.ResourceAllocations.FindAsync(new object[] { id }, cancellationToken);
            if (allocation is null) return;
            allocation.IsDeleted = true;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<ResourceAllocation>> GetActiveByEmployeeIdAsync(int employeeId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ResourceAllocations
                .AsNoTracking()
                .Include(ra => ra.Project)
                .Where(ra => ra.EmployeeId == employeeId && !ra.IsDeleted && ra.AllocationStatus != "Cancelled" && ra.AllocationStatus != "Released")
                .ToListAsync(cancellationToken);
        }
    }
}
