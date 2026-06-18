using AutoMapper;
using HRMS.Api.DTOs.EmployeeDtos;
using HRMS.Api.Models;
using HRMS.Api.Repositories.Interfaces;


namespace HRMS.Api.Services.Interfaces
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;

        public EmployeeService(IEmployeeRepository employeeRepository, IMapper mapper)
        {
            _employeeRepository = employeeRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<IEnumerable<EmployeeDto>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var employees = await _employeeRepository.GetAllAsync(cancellationToken);

            var result = _mapper.Map<IEnumerable<EmployeeDto>>(employees);

            return ApiResponse<IEnumerable<EmployeeDto>>.Ok(result);
        }

        public async Task<ApiResponse<EmployeeDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var employee = await _employeeRepository.GetByIdAsync(id, cancellationToken);

            if (employee is null)
                return ApiResponse<EmployeeDto>.Fail("Employee not found");

            var result = _mapper.Map<EmployeeDto>(employee);

            return ApiResponse<EmployeeDto>.Ok(result);
        }

        public async Task<ApiResponse<EmployeeDto>> CreateAsync(CreateEmployeeDto request, CancellationToken cancellationToken = default)
        {
            var employee = _mapper.Map<Employee>(request);

            // Optional business logic
            employee.CompanyExperience = 0;
            employee.TotalExperience = employee.PriorExperience;

            var created = await _employeeRepository.CreateAsync(employee, cancellationToken);

            var result = _mapper.Map<EmployeeDto>(created);

            return ApiResponse<EmployeeDto>.Ok(result, "Employee created successfully");
        }

        public async Task<ApiResponse<EmployeeDto>> UpdateAsync(int id, UpdateEmployeeDto request, CancellationToken cancellationToken = default)
        {
            var existing = await _employeeRepository.GetByIdAsync(id, cancellationToken);

            if (existing is null)
                return ApiResponse<EmployeeDto>.Fail("Employee not found");

            _mapper.Map(request, existing);

            // Optional business logic recalculation
            existing.TotalExperience = existing.PriorExperience + existing.CompanyExperience;

            var updated = await _employeeRepository.UpdateAsync(existing, cancellationToken);

            var result = _mapper.Map<EmployeeDto>(updated);

            return ApiResponse<EmployeeDto>.Ok(result, "Employee updated successfully");
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var existing = await _employeeRepository.GetByIdAsync(id, cancellationToken);

            if (existing is null)
                return ApiResponse<bool>.Fail("Employee not found");

            await _employeeRepository.DeleteAsync(id, cancellationToken);

            return ApiResponse<bool>.Ok(true, "Employee deleted successfully");
        }
    }
}