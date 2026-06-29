using AutoMapper;
using HRMS.Api.DTOs.MasterDtos;
using HRMS.Api.Models;
using HRMS.Api.Repositories.Interfaces;
using HRMS.Api.Services.Interfaces;

namespace HRMS.Api.Services
{
    public class ProjectStatusService : IProjectStatusService
    {
        private readonly IProjectStatusRepository _repository;
        private readonly IMapper _mapper;

        public ProjectStatusService(IProjectStatusRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<IEnumerable<MasterDto>>> GetAllActiveAsync(CancellationToken ct = default)
        {
            var statuses = await _repository.GetAllActiveAsync(ct);
            var result = _mapper.Map<IEnumerable<MasterDto>>(statuses);
            return ApiResponse<IEnumerable<MasterDto>>.Ok(result);
        }
    }
}
