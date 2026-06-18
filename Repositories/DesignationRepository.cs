using HRMS.Api.Data;
using HRMS.Api.Models;
using HRMS.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Repositories
{
    public class DesignationRepository : IDesignationRepository
    {
        private readonly AppDbContext _dbContext;

        public DesignationRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Designation>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.Designations
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<Designation?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.Designations
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);
        }

        public async Task<Designation?> GetByNameAsync(
            string name,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.Designations
                .FirstOrDefaultAsync(
                    x => x.Name == name,
                    cancellationToken);
        }

        public async Task<Designation> CreateAsync(
            Designation designation,
            CancellationToken cancellationToken = default)
        {
            _dbContext.Designations.Add(designation);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return designation;
        }

        public async Task<Designation> UpdateAsync(
            Designation designation,
            CancellationToken cancellationToken = default)
        {
            _dbContext.Designations.Update(designation);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return designation;
        }

        public async Task DeleteAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var designation = await _dbContext.Designations
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

            if (designation is null)
                return;

            _dbContext.Designations.Remove(designation);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}