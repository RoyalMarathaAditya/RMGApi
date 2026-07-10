using HRMS.Api.Data;
using HRMS.Api.DTOs.Common;
using HRMS.Api.DTOs.UserDtos;
using HRMS.Api.Models;
using HRMS.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Repositories.UserManagement
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _dbContext;

        public UserRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedResponse<UserListDto>> GetPagedAsync(PaginationParams pagination, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.Users
                .AsNoTracking()
                .Include(u => u.Role)
                .Where(u => !u.IsDeleted);

            if (!string.IsNullOrWhiteSpace(pagination.SearchTerm))
            {
                var term = pagination.SearchTerm.ToLower();
                query = query.Where(u =>
                    u.Name.ToLower().Contains(term) ||
                    (u.UserName != null && u.UserName.ToLower().Contains(term)) ||
                    u.Email.ToLower().Contains(term));
            }

            if (!string.IsNullOrWhiteSpace(pagination.RoleIdFilter) && Guid.TryParse(pagination.RoleIdFilter, out var roleId))
            {
                query = query.Where(u => u.RoleId == roleId);
            }

            if (!string.IsNullOrWhiteSpace(pagination.StatusFilter))
            {
                var status = pagination.StatusFilter.ToLower();
                if (status == "active")
                    query = query.Where(u => u.IsActive);
                else if (status == "inactive")
                    query = query.Where(u => !u.IsActive);
                else if (status == "locked")
                    query = query.Where(u => u.IsLocked);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            query = (pagination.SortBy?.ToLower()) switch
            {
                "name" => pagination.SortDescending ? query.OrderByDescending(u => u.Name) : query.OrderBy(u => u.Name),
                "username" => pagination.SortDescending ? query.OrderByDescending(u => u.UserName ?? "") : query.OrderBy(u => u.UserName ?? ""),
                "role" => pagination.SortDescending ? query.OrderByDescending(u => u.Role != null ? u.Role.Name : "") : query.OrderBy(u => u.Role != null ? u.Role.Name : ""),
                "createdat" => pagination.SortDescending ? query.OrderByDescending(u => u.CreatedAt) : query.OrderBy(u => u.CreatedAt),
                "lastlogindate" => pagination.SortDescending ? query.OrderByDescending(u => u.LastLoginDate) : query.OrderBy(u => u.LastLoginDate),
                _ => query.OrderBy(u => u.Name)
            };

            var items = await query
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .Select(u => new UserListDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    UserName = u.UserName,
                    Email = u.Email,
                    Phone = u.Phone,
                    RoleId = u.RoleId,
                    RoleName = u.Role != null ? u.Role.Name : string.Empty,
                    EmployeeId = u.EmployeeId,
                    IsActive = u.IsActive,
                    IsLocked = u.IsLocked,
                    LastLoginDate = u.LastLoginDate,
                    CreatedAt = u.CreatedAt,
                    CreatedBy = u.CreatedBy,
                    ModifiedBy = u.ModifiedBy,
                    ModifiedOn = u.ModifiedOn,
                    FailedLoginCount = u.FailedLoginCount,
                    LockedDate = u.LockedDate,
                    LockedBy = u.LockedBy
                })
                .ToListAsync(cancellationToken);

            return new PagedResponse<UserListDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };
        }

        public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Users
                .Include(u => u.Role)
                .Include(u => u.Employee)
                    .ThenInclude(e => e!.Designation)
                .Include(u => u.Employee)
                    .ThenInclude(e => e!.Practice)
                .Include(u => u.Employee)
                    .ThenInclude(e => e!.DepartmentType)
                .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted, cancellationToken);
        }

        public async Task<User?> GetByEmployeeIdAsync(int employeeId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Users
                .FirstOrDefaultAsync(u => u.EmployeeId == employeeId && !u.IsDeleted, cancellationToken);
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted, cancellationToken);
        }

        public async Task<bool> IsUserNameUniqueAsync(string userName, int? excludeId = null, CancellationToken cancellationToken = default)
        {
            if (excludeId.HasValue)
                return !await _dbContext.Users.AnyAsync(u => u.UserName == userName && u.Id != excludeId.Value && !u.IsDeleted, cancellationToken);
            return !await _dbContext.Users.AnyAsync(u => u.UserName == userName && !u.IsDeleted, cancellationToken);
        }

        public async Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null, CancellationToken cancellationToken = default)
        {
            if (excludeId.HasValue)
                return !await _dbContext.Users.AnyAsync(u => u.Email == email && u.Id != excludeId.Value && !u.IsDeleted, cancellationToken);
            return !await _dbContext.Users.AnyAsync(u => u.Email == email && !u.IsDeleted, cancellationToken);
        }

        public async Task<bool> IsPhoneUniqueAsync(string phone, int? excludeId = null, CancellationToken cancellationToken = default)
        {
            if (excludeId.HasValue)
                return !await _dbContext.Users.AnyAsync(u => u.Phone == phone && u.Id != excludeId.Value && !u.IsDeleted, cancellationToken);
            return !await _dbContext.Users.AnyAsync(u => u.Phone == phone && !u.IsDeleted, cancellationToken);
        }

        public async Task<List<Employee>> GetEmployeesWithoutUserAsync(CancellationToken cancellationToken = default)
        {
            var usedIds = await _dbContext.Users
                .Where(u => u.EmployeeId != null && !u.IsDeleted)
                .Select(u => u.EmployeeId!.Value)
                .ToListAsync(cancellationToken);

            return await _dbContext.Employees
                .AsNoTracking()
                .Where(e => !e.IsDeleted && !usedIds.Contains(e.Id))
                .OrderBy(e => e.FullName)
                .ToListAsync(cancellationToken);
        }

        public async Task<User> CreateAsync(User user, CancellationToken cancellationToken = default)
        {
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return user;
        }

        public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
        {
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(User user, CancellationToken cancellationToken = default)
        {
            user.IsDeleted = true;
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}