using HRMS.Api.Data;
using HRMS.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext _dbContext;

        public RefreshTokenRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(RefreshToken token, CancellationToken cancellationToken = default)
        {
            await _dbContext.RefreshTokens.AddAsync(token, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
        {
            return await _dbContext.RefreshTokens.AsNoTracking().FirstOrDefaultAsync(t => t.Token == token, cancellationToken);
        }

        public async Task DeleteAsync(RefreshToken token, CancellationToken cancellationToken = default)
        {
            _dbContext.RefreshTokens.Remove(token);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
