using HRMS.Api.DTOs.EmployeeDtos;
using HRMS.Api.Models;

namespace HRMS.Api.Services
{
    public interface IEmployeeService
    {
        Task<ApiResponse<IEnumerable<EmployeeDto>>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<EmployeeDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<ApiResponse<EmployeeDto>> CreateAsync(CreateEmployeeDto request, CancellationToken cancellationToken = default);

        Task<ApiResponse<EmployeeDto>> UpdateAsync(int id, UpdateEmployeeDto request, CancellationToken cancellationToken = default);

        Task<ApiResponse<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}