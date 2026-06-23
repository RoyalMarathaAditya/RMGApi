using AutoMapper;
using HRMS.Api.DTOs.LeaveDtos;
using HRMS.Api.Models;
using HRMS.Api.Repositories.Interfaces;
using HRMS.Api.Services.Interfaces;

namespace HRMS.Api.Services
{
    public class LeaveService : ILeaveService
    {
        private readonly ILeaveRepository _repository;
        private readonly IMapper _mapper;

        public LeaveService(ILeaveRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<LeaveDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var leaves = await _repository.GetAllAsync(cancellationToken);
            return _mapper.Map<IEnumerable<LeaveDto>>(leaves);
        }

        public async Task<LeaveDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var leave = await _repository.GetByIdAsync(id, cancellationToken);
            return leave is null ? null : _mapper.Map<LeaveDto>(leave);
        }

        public async Task<LeaveDto> CreateAsync(CreateLeaveDto dto, CancellationToken cancellationToken = default)
        {
            var leave = _mapper.Map<EmployeeLeave>(dto);
            var created = await _repository.CreateAsync(leave, cancellationToken);
            return _mapper.Map<LeaveDto>(created);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            await _repository.DeleteAsync(id, cancellationToken);
            return true;
        }
    }
}
