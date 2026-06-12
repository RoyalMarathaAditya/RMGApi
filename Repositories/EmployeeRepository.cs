using HRMS.Api.Data;
using HRMS.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly HrmsDbContext _dbContext;

        public EmployeeRepository(HrmsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Employee>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Employees.AsNoTracking().ToListAsync(cancellationToken);
        }

        public async Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Employees.FindAsync(new object[] { id }, cancellationToken);
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
            var employee = await GetByIdAsync(id, cancellationToken);
            if (employee is null)
            {
                return;
            }

            _dbContext.Employees.Remove(employee);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
