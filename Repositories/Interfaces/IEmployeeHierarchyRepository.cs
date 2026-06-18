using HRMS.Api.Models;

namespace HRMS.Api.Repositories.Interfaces
{
    public interface IEmployeeHierarchyRepository
    {
        Task<IEnumerable<EmployeeHierarchy>> GetAllAsync();
        Task<EmployeeHierarchy> CreateAsync(EmployeeHierarchy entity);
        Task DeleteAsync(int id);
    }
}