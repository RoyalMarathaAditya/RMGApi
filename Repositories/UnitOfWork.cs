using HRMS.Api.Data;

namespace HRMS.Api.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;
        private IEmployeeRepository? _employees;
        private IAuthRepository? _users;
        private IRefreshTokenRepository? _refreshTokens;

        public UnitOfWork(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEmployeeRepository Employees => _employees ??= new EmployeeRepository(_dbContext);
        public IAuthRepository Users => _users ??= new AuthRepository(_dbContext);
        public IRefreshTokenRepository RefreshTokens => _refreshTokens ??= new RefreshTokenRepository(_dbContext);

        public async Task<int> SaveAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
