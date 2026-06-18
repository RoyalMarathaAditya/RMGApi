using HRMS.Api.Models;

namespace HRMS.Api.Repositories.Interfaces
{
    public interface IDesignationRepository
    {
        Task<IEnumerable<Designation>> GetAllAsync(
            CancellationToken cancellationToken = default);

        Task<Designation?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<Designation?> GetByNameAsync(
            string name,
            CancellationToken cancellationToken = default);

        Task<Designation> CreateAsync(
            Designation designation,
            CancellationToken cancellationToken = default);

        Task<Designation> UpdateAsync(
            Designation designation,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(
            int id,
            CancellationToken cancellationToken = default);
    }
}