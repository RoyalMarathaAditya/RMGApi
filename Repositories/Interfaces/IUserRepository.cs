using HRMS.Api.DTOs.Common;
using HRMS.Api.DTOs.UserDtos;
using HRMS.Api.Models;

namespace HRMS.Api.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<PagedResponse<UserListDto>> GetPagedAsync(PaginationParams pagination, CancellationToken cancellationToken = default);
        Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<bool> IsUserNameUniqueAsync(string userName, int? excludeId = null, CancellationToken cancellationToken = default);
        Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null, CancellationToken cancellationToken = default);
        Task<bool> IsPhoneUniqueAsync(string phone, int? excludeId = null, CancellationToken cancellationToken = default);
        Task<List<Employee>> GetEmployeesWithoutUserAsync(CancellationToken cancellationToken = default);
        Task<User> CreateAsync(User user, CancellationToken cancellationToken = default);
        Task UpdateAsync(User user, CancellationToken cancellationToken = default);
        Task DeleteAsync(User user, CancellationToken cancellationToken = default);
    }
}