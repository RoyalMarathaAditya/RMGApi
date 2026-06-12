namespace HRMS.Api.Repositories
{
    public interface IUnitOfWork
    {
        IEmployeeRepository Employees { get; }
        IAuthRepository Users { get; }
        IRefreshTokenRepository RefreshTokens { get; }
        Task<int> SaveAsync(CancellationToken cancellationToken = default);
    }
}
