using HRMS.Api.DTOs.EmployeeDtos;
using HRMS.Api.Models;
using HRMS.Api.Repositories.Interfaces;
using HRMS.Api.Services.Interfaces;

namespace HRMS.Api.Services
{
    public class EmployeeHierarchyService : IEmployeeHierarchyService
    {
        private readonly IEmployeeHierarchyRepository _repo;

        public EmployeeHierarchyService(IEmployeeHierarchyRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<EmployeeHierarchyDto>> GetAllAsync()
        {
            var data = await _repo.GetAllAsync();

            return data.Select(x => new EmployeeHierarchyDto
            {
                EmployeeId = x.EmployeeId,
                EmployeeName = x.Employee.FullName,
                ManagementLeaderId = x.ManagementLeaderId,
                ManagementLeaderName = x.ManagementLeader.FullName,
                ManagerType = x.ManagerType.ToString()
            });
        }

        public async Task<EmployeeHierarchyDto> CreateAsync(CreateEmployeeHierarchyDto request)
        {
            var entity = new EmployeeHierarchy
            {
                EmployeeId = request.EmployeeId,
                ManagementLeaderId = request.ManagerId,
                ManagerType = request.ManagerType
            };

            var result = await _repo.CreateAsync(entity);

            return new EmployeeHierarchyDto
            {
                EmployeeId = result.EmployeeId,
                ManagementLeaderId = result.ManagementLeaderId,
                ManagerType = result.ManagerType.ToString()
            };
        }
    }
}