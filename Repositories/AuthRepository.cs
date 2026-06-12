using HRMS.Api.Data;
using HRMS.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly HrmsDbContext _dbContext;

        public AuthRepository(HrmsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }
    }
}
