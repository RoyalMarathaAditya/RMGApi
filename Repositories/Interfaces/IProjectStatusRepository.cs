using HRMS.Api.Models;

namespace HRMS.Api.Repositories.Interfaces
{
    public interface IProjectStatusRepository
    {
        Task<IEnumerable<ProjectStatusMaster>> GetAllActiveAsync(CancellationToken ct = default);
    }
}
