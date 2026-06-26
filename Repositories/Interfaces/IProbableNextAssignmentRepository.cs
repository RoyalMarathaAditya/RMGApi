using HRMS.Api.Models;

namespace HRMS.Api.Repositories.Interfaces
{
    public interface IProbableNextAssignmentRepository
    {
        Task<IEnumerable<ProbableNextAssignmentMaster>> GetAllActiveAsync(CancellationToken ct = default);
    }
}
