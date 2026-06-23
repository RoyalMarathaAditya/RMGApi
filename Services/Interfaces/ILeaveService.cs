using HRMS.Api.DTOs.LeaveDtos;

namespace HRMS.Api.Services.Interfaces
{
    public interface ILeaveService
    {
        Task<IEnumerable<LeaveDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<LeaveDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<LeaveDto> CreateAsync(CreateLeaveDto dto, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
