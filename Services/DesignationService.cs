using AutoMapper;
using HRMS.Api.DTOs.DesignationDtos;
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

        public async Task<ApiResponse<IEnumerable<DesignationDto>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var data = await _repository.GetAllAsync(cancellationToken);
            return ApiResponse<IEnumerable<DesignationDto>>.Ok(_mapper.Map<IEnumerable<DesignationDto>>(data));
        }

        public async Task<ApiResponse<DesignationDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var data = await _repository.GetByIdAsync(id, cancellationToken);

            if (data == null)
                return ApiResponse<DesignationDto>.Fail("Designation not found");

            return ApiResponse<DesignationDto>.Ok(_mapper.Map<DesignationDto>(data));
        }

        public async Task<ApiResponse<DesignationDto>> CreateAsync(CreateDesignationDto request, CancellationToken cancellationToken = default)
        {
            var entity = _mapper.Map<Designation>(request);
            var created = await _repository.CreateAsync(entity, cancellationToken);

            return ApiResponse<DesignationDto>.Ok(_mapper.Map<DesignationDto>(created), "Designation created successfully");
        }

        public async Task<ApiResponse<DesignationDto>> UpdateAsync(int id, UpdateDesignationDto request, CancellationToken cancellationToken = default)
        {
            var existing = await _repository.GetByIdAsync(id, cancellationToken);

            if (existing == null)
                return ApiResponse<DesignationDto>.Fail("Designation not found");

            _mapper.Map(request, existing);

            var updated = await _repository.UpdateAsync(existing, cancellationToken);

            return ApiResponse<DesignationDto>.Ok(_mapper.Map<DesignationDto>(updated), "Designation updated successfully");
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var existing = await _repository.GetByIdAsync(id, cancellationToken);

            if (existing == null)
                return ApiResponse<bool>.Fail("Designation not found");

            await _repository.DeleteAsync(id, cancellationToken);

            return ApiResponse<bool>.Ok(true, "Designation deleted successfully");
        }
    }
}