using AutoMapper;
using HRMS.Api.DTOs.MasterDtos;
using HRMS.Api.Models;
using HRMS.Api.Repositories.Interfaces;
using HRMS.Api.Services.Interfaces;

namespace HRMS.Api.Services
{
    public class OnboardingStatusService : IOnboardingStatusService
    {
        private readonly IOnboardingStatusRepository _repository;
        private readonly IMapper _mapper;

        public OnboardingStatusService(IOnboardingStatusRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<IEnumerable<MasterDto>>> GetAllActiveAsync(CancellationToken ct = default)
        {
            var items = await _repository.GetAllActiveAsync(ct);
            var result = _mapper.Map<IEnumerable<MasterDto>>(items);
            return ApiResponse<IEnumerable<MasterDto>>.Ok(result);
        }
    }
}
