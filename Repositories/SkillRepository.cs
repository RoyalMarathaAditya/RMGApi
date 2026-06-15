using HRMS.Api.Data;
using HRMS.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Repositories
{
    public class SkillRepository : ISkillRepository
    {
        private readonly AppDbContext _dbContext;

        public SkillRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Skill>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Skills.AsNoTracking().ToListAsync(cancellationToken);
        }

        public async Task<Skill?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Skills.FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<Skill> CreateAsync(Skill skill, CancellationToken cancellationToken = default)
        {
            _dbContext.Skills.Add(skill);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return skill;
        }

        public async Task<Skill> UpdateAsync(Skill skill, CancellationToken cancellationToken = default)
        {
            _dbContext.Skills.Update(skill);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return skill;
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var s = await GetByIdAsync(id, cancellationToken);
            if (s is null) return;
            _dbContext.Skills.Remove(s);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
