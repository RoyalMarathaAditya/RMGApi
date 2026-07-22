using AutoMapper;
using AutoMapper.QueryableExtensions;
using HRMS.Api.Data;
using HRMS.Api.DTOs.Common;
using HRMS.Api.DTOs.EmployeeDtos;
using HRMS.Api.Models;
using HRMS.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Services.Interfaces
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public EmployeeService(IEmployeeRepository employeeRepository, AppDbContext dbContext, IMapper mapper)
        {
            _employeeRepository = employeeRepository;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        private IQueryable<Employee> GetBaseQuery()
        {
            return _dbContext.Employees.AsNoTracking().Where(e => !e.IsDeleted);
        }

        public async Task<ApiResponse<IEnumerable<EmployeeDto>>> GetAllAsync(string? fullName = null, Guid? practiceId = null, DateTime? doj = null, Guid? statusId = null, CancellationToken cancellationToken = default)
        {
            var query = GetBaseQuery();

            if (!string.IsNullOrWhiteSpace(fullName))
                query = query.Where(x => x.FullName.Contains(fullName));

            if (practiceId.HasValue)
                query = query.Where(x => x.PracticeId == practiceId.Value);

            if (doj.HasValue)
                query = query.Where(x => x.DOJ.Year == doj.Value.Year && x.DOJ.Month == doj.Value.Month);

            if (statusId.HasValue)
                query = query.Where(x => x.StatusId == statusId.Value);

            var result = await query
                .ProjectTo<EmployeeDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return ApiResponse<IEnumerable<EmployeeDto>>.Ok(result);
        }

        public async Task<ApiResponse<EmployeeDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var result = await GetBaseQuery()
                .Where(e => e.Id == id)
                .ProjectTo<EmployeeDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);

            if (result is null)
                return ApiResponse<EmployeeDto>.Fail("Employee not found");

            return ApiResponse<EmployeeDto>.Ok(result);
        }

        public async Task<ApiResponse<EmployeeDto>> CreateAsync(CreateEmployeeDto request, CancellationToken cancellationToken = default)
        {
            var employee = _mapper.Map<Employee>(request);

            if (request.SkillIds.Any())
            {
                foreach (var skillId in request.SkillIds)
                    employee.EmployeeSkills.Add(new EmployeeSkill { SkillId = skillId });
            }

            var created = await _employeeRepository.CreateAsync(employee, cancellationToken);
            var result = await GetBaseQuery()
                .Where(e => e.Id == created.Id)
                .ProjectTo<EmployeeDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);
            return ApiResponse<EmployeeDto>.Ok(result!, "Employee created successfully");
        }

        public async Task<ApiResponse<EmployeeDto>> UpdateAsync(int id, UpdateEmployeeDto request, CancellationToken cancellationToken = default)
        {
            var existing = await _dbContext.Employees
                .Include(e => e.EmployeeSkills)
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

            if (existing is null)
                return ApiResponse<EmployeeDto>.Fail("Employee not found");

            _mapper.Map(request, existing);

            _dbContext.EmployeeSkills.RemoveRange(existing.EmployeeSkills);
            existing.EmployeeSkills.Clear();
            if (request.SkillIds.Any())
            {
                foreach (var skillId in request.SkillIds)
                    existing.EmployeeSkills.Add(new EmployeeSkill { EmployeeId = id, SkillId = skillId });
            }

            await _employeeRepository.UpdateAsync(existing, cancellationToken);
            var result = await GetBaseQuery()
                .Where(e => e.Id == id)
                .ProjectTo<EmployeeDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);
            return ApiResponse<EmployeeDto>.Ok(result!, "Employee updated successfully");
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var existing = await _dbContext.Employees.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
            if (existing is null)
                return ApiResponse<bool>.Fail("Employee not found");

            existing.IsDeleted = true;
            await _dbContext.SaveChangesAsync(cancellationToken);
            return ApiResponse<bool>.Ok(true, "Employee deleted successfully");
        }

        public async Task<ApiResponse<PagedResponse<EmployeeDto>>> GetPagedAsync(PaginationParams parameters, CancellationToken cancellationToken = default)
        {
            var query = GetBaseQuery();

            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                var search = parameters.SearchTerm.ToLower();
                query = query.Where(e =>
                    e.FullName.ToLower().Contains(search) ||
                    e.Email.ToLower().Contains(search) ||
                    e.EmployeeCode.ToLower().Contains(search) ||
                    (e.MobileNumber != null && e.MobileNumber.Contains(search)));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            if (!string.IsNullOrWhiteSpace(parameters.SortBy))
            {
                query = parameters.SortDescending
                    ? query.OrderByDescending(e => EF.Property<object>(e, parameters.SortBy))
                    : query.OrderBy(e => EF.Property<object>(e, parameters.SortBy));
            }
            else
            {
                query = query.OrderBy(e => e.FullName);
            }

            var items = await query
                .ProjectTo<EmployeeDto>(_mapper.ConfigurationProvider)
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync(cancellationToken);

            return ApiResponse<PagedResponse<EmployeeDto>>.Ok(new PagedResponse<EmployeeDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize
            });
        }

        public async Task<List<EmployeeDropdownDto>> GetDropdownAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Employees
                .AsNoTracking()
                .Where(e => !e.IsDeleted && e.EmployeeStatus!.Name == "Active")
                .OrderBy(e => e.FullName)
                .Select(e => new EmployeeDropdownDto
                {
                    EmployeeId = e.Id,
                    EmployeeCode = e.EmployeeCode,
                    EmployeeName = e.FullName,
                    Email = e.Email,
                    MobileNumber = e.MobileNumber
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<ApiResponse<IEnumerable<EmployeeDto>>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return ApiResponse<IEnumerable<EmployeeDto>>.Ok(Enumerable.Empty<EmployeeDto>());

            var search = searchTerm.ToLower();
            var result = await GetBaseQuery()
                .Where(e =>
                    e.FullName.ToLower().Contains(search) ||
                    e.Email.ToLower().Contains(search) ||
                    e.EmployeeCode.ToLower().Contains(search))
                .ProjectTo<EmployeeDto>(_mapper.ConfigurationProvider)
                .Take(20)
                .ToListAsync(cancellationToken);

            return ApiResponse<IEnumerable<EmployeeDto>>.Ok(result);
        }
    }
}
