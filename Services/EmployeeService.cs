using HRMS.Api.DTOs;
using HRMS.Api.Models;
using HRMS.Api.Repositories;

namespace HRMS.Api.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<IEnumerable<EmployeeDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var employees = await _employeeRepository.GetAllAsync(cancellationToken);
            return employees.Select(ToDto);
        }

        public async Task<EmployeeDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var employee = await _employeeRepository.GetByIdAsync(id, cancellationToken);
            return employee is null ? null : ToDto(employee);
        }

        public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto request, CancellationToken cancellationToken = default)
        {
            var employee = new Employee
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Department = request.Department,
                Designation = request.Designation,
                Status = request.Status,
                DateOfJoining = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            var created = await _employeeRepository.CreateAsync(employee, cancellationToken);
            return ToDto(created);
        }

        public async Task<EmployeeDto?> UpdateAsync(int id, UpdateEmployeeDto request, CancellationToken cancellationToken = default)
        {
            var existing = await _employeeRepository.GetByIdAsync(id, cancellationToken);
            if (existing is null)
            {
                return null;
            }

            existing.FirstName = request.FirstName;
            existing.LastName = request.LastName;
            existing.Email = request.Email;
            existing.Department = request.Department;
            existing.Designation = request.Designation;
            existing.Status = request.Status;
            existing.UpdatedAt = DateTime.UtcNow;

            var updated = await _employeeRepository.UpdateAsync(existing, cancellationToken);
            return ToDto(updated);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            await _employeeRepository.DeleteAsync(id, cancellationToken);
        }

        private static EmployeeDto ToDto(Employee employee) => new()
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email,
            Department = employee.Department,
            Designation = employee.Designation,
            Status = employee.Status,
            DateOfJoining = employee.DateOfJoining,
        };
    }
}
