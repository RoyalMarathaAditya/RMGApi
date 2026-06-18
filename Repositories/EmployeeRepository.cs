using HRMS.Api.Data;
using HRMS.Api.Models;
using HRMS.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext _dbContext;

        public EmployeeRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

       

        public async Task<IEnumerable<Employee>> GetAllAsync(CancellationToken cancellationToken = default)
{
    return await _dbContext.Employees
        .AsNoTracking()
        .Include(e => e.Designation)
        .Include(e => e.Practice)
        .Include(e => e.SubPractice)
        .Include(e => e.Location)
        .Include(e => e.Manager)
        .ToListAsync(cancellationToken);
}

        public async Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Employees
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<Employee> CreateAsync(Employee employee, CancellationToken cancellationToken = default)
        {
            _dbContext.Employees.Add(employee);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return employee;
        }

        public async Task<Employee> UpdateAsync(Employee employee, CancellationToken cancellationToken = default)
        {
            _dbContext.Employees.Update(employee);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return employee;
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var employee = await _dbContext.Employees.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (employee is null) return;

            _dbContext.Employees.Remove(employee);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}