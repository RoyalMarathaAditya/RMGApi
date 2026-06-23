using AutoMapper;
using HRMS.Api.DTOs.PIPDtos;
using HRMS.Api.Models;
using HRMS.Api.Repositories.Interfaces;
using HRMS.Api.Services.Interfaces;

namespace HRMS.Api.Services
{
    public class PipService : IPipService
    {
        private readonly IPipRepository _repository;
        private readonly IMapper _mapper;

        public PipService(IPipRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PipDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var pips = await _repository.GetAllAsync(cancellationToken);
            return _mapper.Map<IEnumerable<PipDto>>(pips);
        }

        public async Task<PipDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var pip = await _repository.GetByIdAsync(id, cancellationToken);
            return pip is null ? null : _mapper.Map<PipDto>(pip);
        }

        public async Task<PipDto> CreateAsync(CreatePipDto dto, CancellationToken cancellationToken = default)
        {
            var pip = _mapper.Map<PIP>(dto);
            var created = await _repository.CreateAsync(pip, cancellationToken);
            return _mapper.Map<PipDto>(created);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            await _repository.DeleteAsync(id, cancellationToken);
            return true;
        }
    }
}
