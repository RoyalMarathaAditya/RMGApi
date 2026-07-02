using HRMS.Api.DTOs;
using HRMS.Api.Models;
using HRMS.Api.Repositories.Interfaces;
using HRMS.Api.Services.Interfaces;

namespace HRMS.Api.Services
{
    public class ColumnValueMappingService : IColumnValueMappingService
    {
        private readonly IColumnValueMappingRepository _repository;

        public ColumnValueMappingService(IColumnValueMappingRepository repository)
        {
            _repository = repository;
        }

        public async Task<ApiResponse<IEnumerable<ColumnValueMappingDto>>> GetAllAsync(CancellationToken ct = default)
        {
            var mappings = await _repository.GetAllAsync(ct);
            var result = mappings.Select(MapToDto);
            return ApiResponse<IEnumerable<ColumnValueMappingDto>>.Ok(result);
        }

        public async Task<ApiResponse<ColumnValueMappingDto>> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var mapping = await _repository.GetByIdAsync(id, ct);
            if (mapping is null)
                return ApiResponse<ColumnValueMappingDto>.Fail("Value mapping not found");
            return ApiResponse<ColumnValueMappingDto>.Ok(MapToDto(mapping));
        }

        public async Task<ApiResponse<ColumnValueMappingDto>> CreateAsync(CreateColumnValueMappingDto request, CancellationToken ct = default)
        {
            var mapping = new ColumnValueMapping
            {
                Id = Guid.NewGuid(),
                TargetProperty = request.TargetProperty,
                SourceValue = request.SourceValue,
                TargetValue = request.TargetValue,
                IsActive = request.IsActive,
                CreatedOn = DateTime.UtcNow
            };
            var created = await _repository.CreateAsync(mapping, ct);
            return ApiResponse<ColumnValueMappingDto>.Ok(MapToDto(created), "Value mapping created successfully");
        }

        public async Task<ApiResponse<ColumnValueMappingDto>> UpdateAsync(Guid id, UpdateColumnValueMappingDto request, CancellationToken ct = default)
        {
            var existing = await _repository.GetByIdAsync(id, ct);
            if (existing is null)
                return ApiResponse<ColumnValueMappingDto>.Fail("Value mapping not found");

            existing.TargetProperty = request.TargetProperty;
            existing.SourceValue = request.SourceValue;
            existing.TargetValue = request.TargetValue;
            existing.IsActive = request.IsActive;
            existing.ModifiedOn = DateTime.UtcNow;

            var updated = await _repository.UpdateAsync(existing, ct);
            return ApiResponse<ColumnValueMappingDto>.Ok(MapToDto(updated), "Value mapping updated successfully");
        }

        public async Task<ApiResponse<bool>> DeleteAsync(Guid id, CancellationToken ct = default)
        {
            var existing = await _repository.GetByIdAsync(id, ct);
            if (existing is null)
                return ApiResponse<bool>.Fail("Value mapping not found");
            await _repository.DeleteAsync(existing, ct);
            return ApiResponse<bool>.Ok(true, "Value mapping deleted successfully");
        }

        private static ColumnValueMappingDto MapToDto(ColumnValueMapping m) => new()
        {
            Id = m.Id,
            TargetProperty = m.TargetProperty,
            SourceValue = m.SourceValue,
            TargetValue = m.TargetValue,
            IsActive = m.IsActive,
            CreatedOn = m.CreatedOn,
            ModifiedOn = m.ModifiedOn
        };
    }
}
