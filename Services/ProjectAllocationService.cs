using AutoMapper;
using HRMS.Api.DTOs.ProjectAllocationDtos;
using HRMS.Api.Models;
using HRMS.Api.Repositories.Interfaces;
using HRMS.Api.Services.Interfaces;

namespace HRMS.Api.Services
{
    public class ProjectAllocationService : IProjectAllocationService
    {
        private readonly IProjectAllocationRepository _repository;
        private readonly IMapper _mapper;

        public ProjectAllocationService(IProjectAllocationRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AllocationDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var allocations = await _repository.GetAllAsync(cancellationToken);
            return _mapper.Map<IEnumerable<AllocationDto>>(allocations);
        }

        public async Task<IEnumerable<AllocationDto>> GetByEmployeeIdAsync(int employeeId, CancellationToken cancellationToken = default)
        {
            var allocations = await _repository.GetByEmployeeIdAsync(employeeId, cancellationToken);
            return _mapper.Map<IEnumerable<AllocationDto>>(allocations);
        }

        public async Task<IEnumerable<AllocationDto>> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken = default)
        {
            var allocations = await _repository.GetByProjectIdAsync(projectId, cancellationToken);
            return _mapper.Map<IEnumerable<AllocationDto>>(allocations);
        }

        public async Task<AllocationDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var allocation = await _repository.GetByIdAsync(id, cancellationToken);
            return allocation is null ? null : _mapper.Map<AllocationDto>(allocation);
        }

        public async Task<AllocationDto> CreateAsync(CreateAllocationDto dto, CancellationToken cancellationToken = default)
        {
            var allocation = _mapper.Map<ProjectAllocation>(dto);
            var created = await _repository.CreateAsync(allocation, cancellationToken);
            return _mapper.Map<AllocationDto>(created);
        }

        public async Task<AllocationDto> UpdateAsync(int id, UpdateAllocationDto dto, CancellationToken cancellationToken = default)
        {
            var existing = await _repository.GetByIdAsync(id, cancellationToken);
            if (existing is null)
                throw new KeyNotFoundException($"Allocation with Id {id} not found");

            _mapper.Map(dto, existing);
            var updated = await _repository.UpdateAsync(existing, cancellationToken);
            return _mapper.Map<AllocationDto>(updated);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            await _repository.DeleteAsync(id, cancellationToken);
            return true;
        }

        public async Task<bool> ReleaseResourceAsync(int id, CancellationToken cancellationToken = default)
        {
            var allocation = await _repository.GetByIdAsync(id, cancellationToken);
            if (allocation is null)
                throw new KeyNotFoundException($"Allocation with Id {id} not found");

            allocation.AllocationEndDate = DateTime.UtcNow;
            await _repository.UpdateAsync(allocation, cancellationToken);
            return true;
        }
    }
}
