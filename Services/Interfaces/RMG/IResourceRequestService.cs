using HRMS.Api.DTOs.ResourceRequestDtos;

namespace HRMS.Api.Services.Interfaces.RMG
{
    public interface IResourceRequestService
    {
        Task<IEnumerable<ResourceRequestDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<ResourceRequestDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<ResourceRequestDto> CreateAsync(CreateResourceRequestDto dto, int requestedById, CancellationToken cancellationToken = default);
        Task<ResourceRequestDto?> UpdateStatusAsync(int id, string status, string? notes, string userName, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
