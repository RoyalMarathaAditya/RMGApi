using AutoMapper;
using HRMS.Api.DTOs;
using HRMS.Api.DTOs.Common;
using HRMS.Api.Models;
using HRMS.Api.Repositories.Interfaces;
using HRMS.Api.Services.Interfaces;

namespace HRMS.Api.Services
{
    public class DesignationService : IDesignationService
    {
        private readonly IDesignationRepository _repository;
        private readonly IMapper _mapper;

        public DesignationService(IDesignationRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<IEnumerable<DesignationDto>>> GetAllAsync(CancellationToken ct = default)
        {
            var designations = await _repository.GetAllAsync(ct);
            var result = _mapper.Map<IEnumerable<DesignationDto>>(designations);
            return ApiResponse<IEnumerable<DesignationDto>>.Ok(result);
        }

        public async Task<ApiResponse<IEnumerable<DesignationDto>>> GetActiveAsync(CancellationToken ct = default)
        {
            var designations = await _repository.GetActiveAsync(ct);
            var result = _mapper.Map<IEnumerable<DesignationDto>>(designations);
            return ApiResponse<IEnumerable<DesignationDto>>.Ok(result);
        }

        public async Task<ApiResponse<DesignationDto>> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var designation = await _repository.GetByIdAsync(id, ct);
            if (designation is null)
                return ApiResponse<DesignationDto>.Fail("Designation not found");

            var result = _mapper.Map<DesignationDto>(designation);
            return ApiResponse<DesignationDto>.Ok(result);
        }

        public async Task<ApiResponse<DesignationDto>> CreateAsync(CreateDesignationDto request, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return ApiResponse<DesignationDto>.Fail("Name is required");

            if (await _repository.ExistsAsync(request.Name, ct))
                return ApiResponse<DesignationDto>.Fail($"Designation '{request.Name}' already exists");

            if (!string.IsNullOrWhiteSpace(request.Code) && await _repository.CodeExistsAsync(request.Code, ct))
                return ApiResponse<DesignationDto>.Fail($"Code '{request.Code}' already exists");

            var designation = new DesignationMaster
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Code = request.Code,
                SortOrder = request.SortOrder,
                IsActive = true,
                CreatedOn = DateTime.UtcNow
            };

            var created = await _repository.CreateAsync(designation, ct);
            var result = _mapper.Map<DesignationDto>(created);
            return ApiResponse<DesignationDto>.Ok(result, "Designation created successfully");
        }

        public async Task<ApiResponse<DesignationDto>> UpdateAsync(Guid id, UpdateDesignationDto request, CancellationToken ct = default)
        {
            var existing = await _repository.GetByIdAsync(id, ct);
            if (existing is null)
                return ApiResponse<DesignationDto>.Fail("Designation not found");

            if (string.IsNullOrWhiteSpace(request.Name))
                return ApiResponse<DesignationDto>.Fail("Name is required");

            if (await _repository.ExistsAsync(request.Name, ct) && !string.Equals(existing.Name, request.Name, StringComparison.OrdinalIgnoreCase))
                return ApiResponse<DesignationDto>.Fail($"Designation '{request.Name}' already exists");

            if (!string.IsNullOrWhiteSpace(request.Code) && await _repository.CodeExistsAsync(request.Code, ct) && !string.Equals(existing.Code, request.Code, StringComparison.OrdinalIgnoreCase))
                return ApiResponse<DesignationDto>.Fail($"Code '{request.Code}' already exists");

            existing.Name = request.Name;
            existing.Code = request.Code;
            existing.IsActive = request.IsActive;
            existing.SortOrder = request.SortOrder;
            existing.ModifiedOn = DateTime.UtcNow;

            var updated = await _repository.UpdateAsync(existing, ct);
            var result = _mapper.Map<DesignationDto>(updated);
            return ApiResponse<DesignationDto>.Ok(result, "Designation updated successfully");
        }

        public async Task<ApiResponse<bool>> DeleteAsync(Guid id, CancellationToken ct = default)
        {
            var existing = await _repository.GetByIdAsync(id, ct);
            if (existing is null)
                return ApiResponse<bool>.Fail("Designation not found");

            await _repository.DeleteAsync(existing, ct);
            return ApiResponse<bool>.Ok(true, "Designation deleted successfully");
        }
    }
}