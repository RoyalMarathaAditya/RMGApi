using HRMS.Api.Models;

namespace HRMS.Api.Repositories.Interfaces
{
    public interface ILeaveRepository
    {
        Task<IEnumerable<EmployeeLeave>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<EmployeeLeave?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<EmployeeLeave> CreateAsync(EmployeeLeave leave, CancellationToken cancellationToken = default);
        Task<EmployeeLeave> UpdateAsync(EmployeeLeave leave, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
