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
                .Include(e => e.EmploymentType)
                .Include(e => e.Location)
                .Include(e => e.WorkModel)
                .Include(e => e.Practice)
                .Include(e => e.DepartmentType)
                .Include(e => e.EmployeeStatus)
                .Include(e => e.ReportingManager)
                .Include(e => e.PracticeHead)
                .Include(e => e.EmployeeSkills).ThenInclude(es => es.Skill)
                .ToListAsync(cancellationToken);
        }

        public async Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Employees
                .Include(e => e.EmploymentType)
                .Include(e => e.Location)
                .Include(e => e.WorkModel)
                .Include(e => e.Practice)
                .Include(e => e.DepartmentType)
                .Include(e => e.EmployeeStatus)
                .Include(e => e.ReportingManager)
                .Include(e => e.PracticeHead)
                .Include(e => e.EmployeeSkills).ThenInclude(es => es.Skill)
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
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
            var employee = await _dbContext.Employees.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
            if (employee is null) return;
            employee.IsDeleted = true;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
