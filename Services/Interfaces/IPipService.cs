using HRMS.Api.DTOs.PIPDtos;

namespace HRMS.Api.Services.Interfaces
{
    public interface IPipService
    {
        Task<IEnumerable<PipDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<PipDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<PipDto> CreateAsync(CreatePipDto dto, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
