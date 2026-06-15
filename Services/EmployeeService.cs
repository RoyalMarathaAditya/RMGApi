using AutoMapper;
using HRMS.Api.DTOs;
using HRMS.Api.Models;
using HRMS.Api.Repositories;

namespace HRMS.Api.Services
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

        public async Task<IEnumerable<EmployeeDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var employees = await _employeeRepository.GetAllAsync(cancellationToken);
            return _mapper.Map<IEnumerable<EmployeeDto>>(employees);
        }

        public async Task<EmployeeDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var employee = await _employeeRepository.GetByIdAsync(id, cancellationToken);
            return employee is null ? null : _mapper.Map<EmployeeDto>(employee);
        }

        public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto request, CancellationToken cancellationToken = default)
        {
            var employee = _mapper.Map<Employee>(request);
            var created = await _employeeRepository.CreateAsync(employee, cancellationToken);
            return _mapper.Map<EmployeeDto>(created);
        }

        public async Task<EmployeeDto?> UpdateAsync(int id, UpdateEmployeeDto request, CancellationToken cancellationToken = default)
        {
            var existing = await _employeeRepository.GetByIdAsync(id, cancellationToken);
            if (existing is null)
            {
                return null;
            }

            _mapper.Map(request, existing);
            var updated = await _employeeRepository.UpdateAsync(existing, cancellationToken);
            return _mapper.Map<EmployeeDto>(updated);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            await _employeeRepository.DeleteAsync(id, cancellationToken);
        }
    }
}
