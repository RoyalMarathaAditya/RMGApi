namespace HRMS.Api.Repositories.Interfaces
{
    public interface IMasterRepository
    {
        Task<IEnumerable<object>> GetAllAsync(string type, CancellationToken cancellationToken = default);
    }
}
