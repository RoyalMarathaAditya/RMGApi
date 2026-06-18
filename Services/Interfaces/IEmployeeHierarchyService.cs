using HRMS.Api.DTOs.EmployeeDtos;

namespace HRMS.Api.Services.Interfaces
{
    public interface IEmployeeHierarchyService
    {
        Task<IEnumerable<EmployeeHierarchyDto>> GetAllAsync();
        Task<EmployeeHierarchyDto> CreateAsync(CreateEmployeeHierarchyDto request);
    }
}