using HRMS.Api.Data;
using HRMS.Api.DTOs.DashboardDtos;
using HRMS.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly AppDbContext _dbContext;

        public DashboardService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<PracticeSummaryDto>> GetPracticeSummaryAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Employees
                .GroupBy(e => new { e.PracticeId, e.Practice.Name })
                .Select(g => new PracticeSummaryDto
                {
                    PracticeId = 0,
                    PracticeName = g.Key.Name,
                    EmployeeCount = g.Count()
                })
                .ToListAsync(cancellationToken);
        }
    }
}
