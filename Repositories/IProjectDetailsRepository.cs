using HRMS.Api.Models;

namespace HRMS.Api.Repositories
{
    public interface IProjectDetailsRepository
    {
        Task<IEnumerable<ProjectDetails>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<ProjectDetails?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<ProjectDetails> CreateAsync(ProjectDetails project, CancellationToken cancellationToken = default);
        Task<ProjectDetails> UpdateAsync(ProjectDetails project, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
