using AutoMapper;
using HRMS.Api.Data;
using HRMS.Api.DTOs;
using HRMS.Api.Models;
using HRMS.Api.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HRMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectRepository _projectRepo;
        private readonly AppDbContext _db;
        private readonly ILogger<ProjectsController> _logger;

        public ProjectsController(IProjectRepository projectRepo, AppDbContext db, ILogger<ProjectsController> logger)
        {
            _projectRepo = projectRepo;
            _db = db;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Fetching projects...");
            var items = await _db.Projects
                .AsNoTracking()
                .Include(p => p.Client)
                .Include(p => p.ProjectType)
                .Include(p => p.PricingType)
                .Include(p => p.Practice)
                .Include(p => p.ProjectManager)
                .Select(p => new
                {
                    p.Id,
                    p.ProjectName,
                    p.Description,
                    p.StartDate,
                    p.EndDate,
                    p.IsActive,
                    ClientName = p.Client.Name,
                    ProjectType = p.ProjectType.Name,
                    PricingType = p.PricingType.Name,
                    Practice = p.Practice.Name,
                    ProjectManager = p.ProjectManager != null ? p.ProjectManager.FullName : null
                })
                .ToListAsync();
            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation("Fetching project with {Id}...", id);
            var p = await _db.Projects
                .AsNoTracking()
                .Include(p => p.Client)
                .Include(p => p.ProjectType)
                .Include(p => p.PricingType)
                .Include(p => p.Practice)
                .Include(p => p.ProjectManager)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (p is null)
            {
                _logger.LogWarning("Project {Id} not found", id);
                return NotFound();
            }
            return Ok(p);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Project project)
        {
            _logger.LogInformation("Creating project...");
            _db.Projects.Add(project);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = project.Id }, project);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Project project)
        {
            _logger.LogInformation("Updating project {Id}...", id);
            var existing = await _db.Projects.FindAsync(id);
            if (existing is null)
            {
                _logger.LogWarning("Project {Id} not found for update", id);
                return NotFound();
            }

            existing.ProjectName = project.ProjectName;
            existing.Description = project.Description;
            existing.ClientId = project.ClientId;
            existing.ProjectTypeId = project.ProjectTypeId;
            existing.PricingTypeId = project.PricingTypeId;
            existing.StartDate = project.StartDate;
            existing.EndDate = project.EndDate;
            existing.IsActive = project.IsActive;
            existing.PracticeId = project.PracticeId;
            existing.ProjectManagerId = project.ProjectManagerId;
            existing.CSMId = project.CSMId;

            await _db.SaveChangesAsync();
            return Ok(existing);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Deleting project {Id}...", id);
            var existing = await _db.Projects.FindAsync(id);
            if (existing is null)
            {
                _logger.LogWarning("Project {Id} not found for deletion", id);
                return NotFound();
            }
            existing.IsDeleted = true;
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
