using HRMS.Api.DTOs;
using HRMS.Api.Models;

namespace HRMS.Api.Services.Interfaces
{
    public interface IClientService
    {
        Task<ApiResponse<IEnumerable<ClientDto>>> GetAllAsync(CancellationToken ct = default);
        Task<ApiResponse<ClientDto>> GetByIdAsync(int id, CancellationToken ct = default);
        Task<ApiResponse<ClientDto>> CreateAsync(CreateClientDto request, string userName, CancellationToken ct = default);
        Task<ApiResponse<ClientDto>> UpdateAsync(int id, UpdateClientDto request, string userName, CancellationToken ct = default);
        Task<ApiResponse<bool>> DeleteAsync(int id, string userName, CancellationToken ct = default);
    }
}
