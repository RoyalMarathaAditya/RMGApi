using HRMS.Api.DTOs;

namespace HRMS.Api.Services
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<EmployeeDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<EmployeeDto> CreateAsync(CreateEmployeeDto request, CancellationToken cancellationToken = default);
        Task<EmployeeDto?> UpdateAsync(int id, UpdateEmployeeDto request, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
