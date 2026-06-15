using HRMS.Api.Models;

namespace HRMS.Api.Repositories.Interfaces
{
    public interface ISkillRepository
    {
        Task<IEnumerable<Skill>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Skill?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Skill> CreateAsync(Skill skill, CancellationToken cancellationToken = default);
        Task<Skill> UpdateAsync(Skill skill, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
