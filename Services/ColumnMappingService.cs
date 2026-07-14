using HRMS.Api.DTOs;
using HRMS.Api.Models;
using HRMS.Api.Repositories.Interfaces;
using HRMS.Api.Services.Interfaces;

namespace HRMS.Api.Services
{
    public class ColumnMappingService : IColumnMappingService
    {
        private readonly IColumnMappingRepository _repository;

        public ColumnMappingService(IColumnMappingRepository repository)
        {
            _repository = repository;
        }

        public async Task<ApiResponse<IEnumerable<ColumnMappingDto>>> GetAllAsync(CancellationToken ct = default)
        {
            var mappings = await _repository.GetAllAsync(ct);
            var result = mappings.Select(MapToDto);
            return ApiResponse<IEnumerable<ColumnMappingDto>>.Ok(result);
        }

        public async Task<ApiResponse<IEnumerable<ColumnMappingDto>>> GetByEntityTypeAsync(string entityType, CancellationToken ct = default)
        {
            var mappings = await _repository.GetByEntityTypeAsync(entityType, true, ct);
            var result = mappings.Select(MapToDto);
            return ApiResponse<IEnumerable<ColumnMappingDto>>.Ok(result);
        }

        public async Task<ApiResponse<ColumnMappingDto>> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var mapping = await _repository.GetByIdAsync(id, ct);
            if (mapping is null)
                return ApiResponse<ColumnMappingDto>.Fail("Column mapping not found");
            return ApiResponse<ColumnMappingDto>.Ok(MapToDto(mapping));
        }

        public async Task<ApiResponse<ColumnMappingDto>> CreateAsync(CreateColumnMappingDto request, CancellationToken ct = default)
        {
            var duplicateCheck = await CheckDuplicatesAsync(request.EntityType, request.SourceColumn, request.TargetDisplayName, null, ct);
            if (duplicateCheck is not null)
                return duplicateCheck;

            var mapping = new ColumnMapping
            {
                Id = Guid.NewGuid(),
                SourceColumn = request.SourceColumn,
                TargetProperty = request.TargetProperty,
                TargetDisplayName = request.TargetDisplayName,
                DataType = request.DataType,
                EntityType = request.EntityType,
                IsRequired = request.IsRequired,
                IsActive = request.IsActive,
                DisplayOrder = request.DisplayOrder,
                CreatedOn = DateTime.UtcNow
            };
            var created = await _repository.CreateAsync(mapping, ct);
            return ApiResponse<ColumnMappingDto>.Ok(MapToDto(created), "Column mapping created successfully");
        }

        public async Task<ApiResponse<ColumnMappingDto>> UpdateAsync(Guid id, UpdateColumnMappingDto request, CancellationToken ct = default)
        {
            var existing = await _repository.GetByIdAsync(id, ct);
            if (existing is null)
                return ApiResponse<ColumnMappingDto>.Fail("Column mapping not found");

            var duplicateCheck = await CheckDuplicatesAsync(request.EntityType, request.SourceColumn, request.TargetDisplayName, id, ct);
            if (duplicateCheck is not null)
                return duplicateCheck;

            existing.SourceColumn = request.SourceColumn;
            existing.TargetProperty = request.TargetProperty;
            existing.TargetDisplayName = request.TargetDisplayName;
            existing.DataType = request.DataType;
            existing.EntityType = request.EntityType;
            existing.IsRequired = request.IsRequired;
            existing.IsActive = request.IsActive;
            existing.DisplayOrder = request.DisplayOrder;
            existing.ModifiedOn = DateTime.UtcNow;

            var updated = await _repository.UpdateAsync(existing, ct);
            return ApiResponse<ColumnMappingDto>.Ok(MapToDto(updated), "Column mapping updated successfully");
        }

        private async Task<ApiResponse<ColumnMappingDto>?> CheckDuplicatesAsync(string entityType, string sourceColumn, string targetDisplayName, Guid? excludeId, CancellationToken ct)
        {
            if (await _repository.ExistsByEntityTypeAndSourceColumnAsync(entityType, sourceColumn, excludeId, ct))
                return ApiResponse<ColumnMappingDto>.Fail($"Source column '{sourceColumn}' already exists in {entityType}");

            if (!string.IsNullOrWhiteSpace(targetDisplayName) &&
                await _repository.ExistsByEntityTypeAndTargetDisplayNameAsync(entityType, targetDisplayName, excludeId, ct))
                return ApiResponse<ColumnMappingDto>.Fail($"Target display name '{targetDisplayName}' already exists in {entityType}");

            return null;
        }

        public async Task<ApiResponse<bool>> DeleteAsync(Guid id, CancellationToken ct = default)
        {
            var existing = await _repository.GetByIdAsync(id, ct);
            if (existing is null)
                return ApiResponse<bool>.Fail("Column mapping not found");
            await _repository.DeleteAsync(existing, ct);
            return ApiResponse<bool>.Ok(true, "Column mapping deleted successfully");
        }

        private static ColumnMappingDto MapToDto(ColumnMapping m) => new()
        {
            Id = m.Id,
            SourceColumn = m.SourceColumn,
            TargetProperty = m.TargetProperty,
            TargetDisplayName = m.TargetDisplayName,
            DataType = m.DataType,
            EntityType = m.EntityType,
            IsRequired = m.IsRequired,
            IsActive = m.IsActive,
            DisplayOrder = m.DisplayOrder,
            CreatedOn = m.CreatedOn,
            ModifiedOn = m.ModifiedOn
        };
    }
}
