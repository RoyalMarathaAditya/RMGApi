using HRMS.Api.Data;
using HRMS.Api.Models;
using HRMS.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Repositories
{
    public class EmployeeHierarchyRepository : IEmployeeHierarchyRepository
    {
        private readonly AppDbContext _context;

        public EmployeeHierarchyRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EmployeeHierarchy>> GetAllAsync()
        {
            return await _context.EmployeeHierarchies
                .Include(x => x.Employee)
                .Include(x => x.ManagementLeader) // ✅ FIXED
                .ToListAsync();
        }

        public async Task<EmployeeHierarchy?> GetByIdAsync(int id)
        {
            return await _context.EmployeeHierarchies
                .Include(x => x.Employee)
                .Include(x => x.ManagementLeader) // ✅ FIXED
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<EmployeeHierarchy> CreateAsync(EmployeeHierarchy entity)
        {
            _context.EmployeeHierarchies.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(int id)
        {
            var item = await _context.EmployeeHierarchies.FindAsync(id);
            if (item != null)
            {
                _context.EmployeeHierarchies.Remove(item);
                await _context.SaveChangesAsync();
            }
        }
    }
}