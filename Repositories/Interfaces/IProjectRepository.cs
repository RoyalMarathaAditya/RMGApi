using HRMS.Api.Models;

namespace HRMS.Api.Repositories.Interfaces
{
    public interface IProjectRepository
    {
        Task<IEnumerable<Project>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Project>> GetActiveProjectsAsync(CancellationToken cancellationToken = default);
        Task<Project?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Project> CreateAsync(Project project, CancellationToken cancellationToken = default);
        Task<Project> UpdateAsync(Project project, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
