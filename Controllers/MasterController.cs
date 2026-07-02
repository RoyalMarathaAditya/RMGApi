using HRMS.Api.Data;
using HRMS.Api.DTOs.MasterDtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HRMS.Api.Controllers
{
    [ApiController]
    [Route("api/master")]
    public class MasterController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ILogger<MasterController> _logger;

        public MasterController(AppDbContext db, ILogger<MasterController> logger)
        {
            _db = db;
            _logger = logger;
        }

        [HttpGet("{type}")]
        public async Task<IActionResult> GetMasterData(string type, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching master data for {Type}...", type);
            IQueryable<MasterDto>? query = type.ToLower() switch
            {
                "roles" => _db.RoleMasters.Where(x => x.IsActive).Select(x => new MasterDto { Id = x.Id, Name = x.Name }),
                "employmenttypes" => _db.EmploymentTypeMasters.Where(x => x.IsActive).Select(x => new MasterDto { Id = x.Id, Name = x.Name }),
                "statuses" => _db.StatusMasters.Where(x => x.IsActive).Select(x => new MasterDto { Id = x.Id, Name = x.Name }),
                "workmodels" => _db.WorkModelMasters.Where(x => x.IsActive).Select(x => new MasterDto { Id = x.Id, Name = x.Name }),
                "locations" => _db.Locations.Where(x => x.IsActive).Select(x => new MasterDto { Id = x.Id, Name = x.Name }),
                "practices" => _db.Practices.Where(x => x.IsActive).Select(x => new MasterDto { Id = x.Id, Name = x.Name }),
                "skills" => _db.Skills.Where(x => x.IsActive).Select(x => new MasterDto { Id = x.Id, Name = x.Name }),
                "leavetypes" => _db.LeaveTypeMasters.Where(x => x.IsActive).Select(x => new MasterDto { Id = x.Id, Name = x.Name }),
                "pricingtypes" => _db.PricingTypeMasters.Where(x => x.IsActive).Select(x => new MasterDto { Id = x.Id, Name = x.Name }),
                "projecttypes" => _db.ProjectTypeMasters.Where(x => x.IsActive).Select(x => new MasterDto { Id = x.Id, Name = x.Name }),
                "departmenttypes" => _db.DepartmentTypeMasters.Where(x => x.IsActive).Select(x => new MasterDto { Id = x.Id, Name = x.Name }),
                "designations" => _db.DesignationMasters.Where(x => x.IsActive).Select(x => new MasterDto { Id = x.Id, Name = x.Name }),
                "csmrevengetypes" => _db.CSMRevenueTypes.Where(x => x.IsActive).Select(x => new MasterDto { Id = x.Id, Name = x.Name }),
                _ => null
            };

            if (query is null)
                return NotFound(new { message = $"Master type '{type}' not found. Valid types: roles, employmenttypes, statuses, workmodels, locations, practices, skills, leavetypes, pricingtypes, projecttypes, departmenttypes, designations, csmrevengetypes" });

            var data = await query.OrderBy(x => x.Name).ToListAsync(cancellationToken);
            return Ok(data);
        }
    }
}
