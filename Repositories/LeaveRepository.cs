using HRMS.Api.Data;
using HRMS.Api.Models;
using HRMS.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Repositories
{
    public class LeaveRepository : ILeaveRepository
    {
        private readonly AppDbContext _dbContext;

        public LeaveRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<EmployeeLeave>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.EmployeeLeaves
                .AsNoTracking()
                .Include(el => el.Employee)
                .Include(el => el.LeaveType)
                .ToListAsync(cancellationToken);
        }

        public async Task<EmployeeLeave?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.EmployeeLeaves
                .Include(el => el.Employee)
                .Include(el => el.LeaveType)
                .FirstOrDefaultAsync(el => el.Id == id, cancellationToken);
        }

        public async Task<EmployeeLeave> CreateAsync(EmployeeLeave leave, CancellationToken cancellationToken = default)
        {
            _dbContext.EmployeeLeaves.Add(leave);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return leave;
        }

        public async Task<EmployeeLeave> UpdateAsync(EmployeeLeave leave, CancellationToken cancellationToken = default)
        {
            _dbContext.EmployeeLeaves.Update(leave);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return leave;
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var leave = await _dbContext.EmployeeLeaves.FindAsync(new object[] { id }, cancellationToken);
            if (leave is null) return;
            leave.IsDeleted = true;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
