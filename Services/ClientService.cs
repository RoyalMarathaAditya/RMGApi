using AutoMapper;
using HRMS.Api.DTOs;
using HRMS.Api.Models;
using HRMS.Api.Repositories.Interfaces;
using HRMS.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<ClientService> _logger;

        public ClientService(IClientRepository repository, IMapper mapper, ILogger<ClientService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponse<IEnumerable<ClientDto>>> GetAllAsync(CancellationToken ct = default)
        {
            var clients = await _repository.GetAllAsync(ct);
            var result = _mapper.Map<IEnumerable<ClientDto>>(clients);
            return ApiResponse<IEnumerable<ClientDto>>.Ok(result);
        }

        public async Task<ApiResponse<ClientDto>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var client = await _repository.GetByIdAsync(id, ct);
            if (client is null)
                return ApiResponse<ClientDto>.Fail("Client not found");

            var result = _mapper.Map<ClientDto>(client);
            return ApiResponse<ClientDto>.Ok(result);
        }

        public async Task<ApiResponse<ClientDto>> CreateAsync(CreateClientDto request, string userName, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return ApiResponse<ClientDto>.Fail("Client name is required");

            request.Name = request.Name.Trim();

            if (await _repository.ExistsAsync(request.Name, ct))
                return ApiResponse<ClientDto>.Fail($"Client '{request.Name}' already exists");

            if (request.ContractEndDate.HasValue && request.ContractStartDate.HasValue &&
                request.ContractEndDate < request.ContractStartDate)
                return ApiResponse<ClientDto>.Fail("Contract End Date cannot be earlier than Contract Start Date");

            if (request.StatusId == Guid.Empty)
                return ApiResponse<ClientDto>.Fail("Status is required");

            if (string.IsNullOrWhiteSpace(request.Location))
                return ApiResponse<ClientDto>.Fail("Location is required");

            var client = new Client
            {
                Name = request.Name,
                ContractStartDate = request.ContractStartDate,
                ContractEndDate = request.ContractEndDate,
                StatusId = request.StatusId,
                Location = request.Location?.Trim(),
                CreatedBy = userName,
                CreatedOn = DateTime.UtcNow
            };

            var created = await _repository.CreateAsync(client, ct);
            var result = _mapper.Map<ClientDto>(created);
            return ApiResponse<ClientDto>.Ok(result, "Client created successfully");
        }

        public async Task<ApiResponse<ClientDto>> UpdateAsync(int id, UpdateClientDto request, string userName, CancellationToken ct = default)
        {
            var existing = await _repository.GetByIdAsync(id, ct);
            if (existing is null)
                return ApiResponse<ClientDto>.Fail("Client not found");

            if (string.IsNullOrWhiteSpace(request.Name))
                return ApiResponse<ClientDto>.Fail("Client name is required");

            request.Name = request.Name.Trim();

            if (await _repository.ExistsAsync(request.Name, ct) &&
                !string.Equals(existing.Name, request.Name, StringComparison.OrdinalIgnoreCase))
                return ApiResponse<ClientDto>.Fail($"Client '{request.Name}' already exists");

            if (request.ContractEndDate.HasValue && request.ContractStartDate.HasValue &&
                request.ContractEndDate < request.ContractStartDate)
                return ApiResponse<ClientDto>.Fail("Contract End Date cannot be earlier than Contract Start Date");

            if (string.IsNullOrWhiteSpace(request.Location))
                return ApiResponse<ClientDto>.Fail("Location is required");

            try
            {
                existing.Name = request.Name;
                existing.ContractStartDate = request.ContractStartDate;
                existing.ContractEndDate = request.ContractEndDate;
                existing.StatusId = request.StatusId;
                existing.Location = request.Location?.Trim();
                existing.ModifiedBy = userName;
                existing.ModifiedOn = DateTime.UtcNow;

                var updated = await _repository.UpdateAsync(existing, request.RowVersion, ct);
                var result = _mapper.Map<ClientDto>(updated);
                return ApiResponse<ClientDto>.Ok(result, "Client updated successfully");
            }
            catch (DbUpdateConcurrencyException)
            {
                return ApiResponse<ClientDto>.Fail("This record was modified by another user. Please refresh and try again.");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id, string userName, CancellationToken ct = default)
        {
            var existing = await _repository.GetByIdAsync(id, ct);
            if (existing is null)
                return ApiResponse<bool>.Fail("Client not found");

            try
            {
                existing.ModifiedBy = userName;
                existing.ModifiedOn = DateTime.UtcNow;
                await _repository.SoftDeleteAsync(existing, ct);
                return ApiResponse<bool>.Ok(true, "Client deleted successfully");
            }
            catch (DbUpdateConcurrencyException)
            {
                return ApiResponse<bool>.Fail("This record was modified by another user. Please refresh and try again.");
            }
        }
    }


}
