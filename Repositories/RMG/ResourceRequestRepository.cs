using HRMS.Api.Data;
using HRMS.Api.Models.RMG;
using HRMS.Api.Repositories.Interfaces.RMG;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Repositories.RMG
{
    public class ResourceRequestRepository : IResourceRequestRepository
    {
        private readonly AppDbContext _dbContext;

        public ResourceRequestRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<ResourceRequest>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.ResourceRequests
                .AsNoTracking()
                .Include(r => r.Project)
                .Include(r => r.RequestedBy)
                .Include(r => r.Practice)
                .OrderByDescending(r => r.CreatedOn)
                .ToListAsync(cancellationToken);
        }

        public async Task<ResourceRequest?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ResourceRequests
                .Include(r => r.Project)
                .Include(r => r.RequestedBy)
                .Include(r => r.Practice)
                .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        }

        public async Task<ResourceRequest> CreateAsync(ResourceRequest request, CancellationToken cancellationToken = default)
        {
            _dbContext.ResourceRequests.Add(request);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return request;
        }

        public async Task<ResourceRequest> UpdateAsync(ResourceRequest request, CancellationToken cancellationToken = default)
        {
            _dbContext.ResourceRequests.Update(request);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return request;
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var request = await _dbContext.ResourceRequests.FindAsync(new object[] { id }, cancellationToken);
            if (request is null) return;
            request.IsDeleted = true;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
