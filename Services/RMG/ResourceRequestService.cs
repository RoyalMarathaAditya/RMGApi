using AutoMapper;
using HRMS.Api.DTOs.ResourceRequestDtos;
using HRMS.Api.Models.RMG;
using HRMS.Api.Repositories.Interfaces.RMG;
using HRMS.Api.Services.Interfaces.RMG;
using Microsoft.EntityFrameworkCore;
using HRMS.Api.Data;

namespace HRMS.Api.Services.RMG
{
    public class ResourceRequestService : IResourceRequestService
    {
        private readonly IResourceRequestRepository _repository;
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public ResourceRequestService(IResourceRequestRepository repository, AppDbContext dbContext, IMapper mapper)
        {
            _repository = repository;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ResourceRequestDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var requests = await _repository.GetAllAsync(cancellationToken);
            return requests.Select(ToDto);
        }

        public async Task<ResourceRequestDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var request = await _repository.GetByIdAsync(id, cancellationToken);
            return request is null ? null : ToDto(request);
        }

        public async Task<ResourceRequestDto> CreateAsync(CreateResourceRequestDto dto, int requestedById, CancellationToken cancellationToken = default)
        {
            var request = new ResourceRequest
            {
                ProjectId = dto.ProjectId,
                RequestedById = requestedById,
                PracticeId = dto.PracticeId,
                RequiredSkillIds = dto.RequiredSkillIds,
                RequiredCount = dto.RequiredCount,
                RequiredByDate = dto.RequiredByDate,
                Priority = dto.Priority,
                Notes = dto.Notes,
                Status = "Submitted",
                CreatedBy = requestedById.ToString()
            };

            var created = await _repository.CreateAsync(request, cancellationToken);
            return ToDto(created);
        }

        public async Task<ResourceRequestDto?> UpdateStatusAsync(int id, string status, string? notes, string userName, CancellationToken cancellationToken = default)
        {
            var request = await _repository.GetByIdAsync(id, cancellationToken);
            if (request is null) return null;

            request.Status = status;
            if (notes is not null) request.Notes = notes;
            request.ModifiedBy = userName;
            request.ModifiedOn = DateTime.UtcNow;

            var updated = await _repository.UpdateAsync(request, cancellationToken);
            return ToDto(updated);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            await _repository.DeleteAsync(id, cancellationToken);
            return true;
        }

        private ResourceRequestDto ToDto(ResourceRequest request)
        {
            var skillNames = new List<string>();
            if (!string.IsNullOrEmpty(request.RequiredSkillIds))
            {
                var ids = request.RequiredSkillIds.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var idStr in ids)
                {
                    if (Guid.TryParse(idStr.Trim(), out var guid))
                    {
                        var skill = _dbContext.Skills.Find(guid);
                        if (skill is not null) skillNames.Add(skill.Name);
                    }
                }
            }

            return new ResourceRequestDto
            {
                Id = request.Id,
                ProjectId = request.ProjectId,
                ProjectName = request.Project?.ProjectName ?? "",
                RequestedById = request.RequestedById,
                RequestedByName = request.RequestedBy?.FullName ?? "",
                PracticeId = request.PracticeId,
                PracticeName = request.Practice?.Name,
                RequiredSkillIds = request.RequiredSkillIds,
                RequiredSkillNames = string.Join(", ", skillNames),
                RequiredCount = request.RequiredCount,
                RequiredByDate = request.RequiredByDate,
                Status = request.Status,
                Priority = request.Priority,
                Notes = request.Notes,
                CreatedOn = request.CreatedOn,
                CreatedBy = request.CreatedBy
            };
        }
    }
}
