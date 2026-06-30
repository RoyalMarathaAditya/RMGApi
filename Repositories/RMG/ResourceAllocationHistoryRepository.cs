using HRMS.Api.Data;
using HRMS.Api.Models.RMG;
using HRMS.Api.Repositories.Interfaces.RMG;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Repositories.RMG
{
    public class ResourceAllocationHistoryRepository : IResourceAllocationHistoryRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<ResourceAllocationHistoryRepository> _logger;

        public ResourceAllocationHistoryRepository(AppDbContext dbContext, ILogger<ResourceAllocationHistoryRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IEnumerable<ResourceAllocationHistory>> GetByAllocationIdAsync(int allocationId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ResourceAllocationHistory
                .AsNoTracking()
                .Where(h => h.ResourceAllocationId == allocationId)
                .OrderByDescending(h => h.ModifiedDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<ResourceAllocationHistory>> GetByEmployeeIdAsync(int employeeId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ResourceAllocationHistory
                .AsNoTracking()
                .Where(h => h.EmployeeId == employeeId)
                .OrderByDescending(h => h.ModifiedDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<ResourceAllocationHistory> CreateAsync(ResourceAllocationHistory history, CancellationToken cancellationToken = default)
        {
            _dbContext.ResourceAllocationHistory.Add(history);
            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogDebug("ResourceAllocationHistory {HistoryId} created for AllocationId={AllocationId}", history.Id, history.ResourceAllocationId);
            return history;
        }
    }
}
