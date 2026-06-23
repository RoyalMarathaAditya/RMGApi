using AutoMapper;
using HRMS.Api.Data;
using HRMS.Api.DTOs;
using HRMS.Api.Models;
using HRMS.Api.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectRepository _projectRepo;
        private readonly AppDbContext _db;

        public ProjectsController(IProjectRepository projectRepo, AppDbContext db)
        {
            _projectRepo = projectRepo;
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
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
            var p = await _db.Projects
                .AsNoTracking()
                .Include(p => p.Client)
                .Include(p => p.ProjectType)
                .Include(p => p.PricingType)
                .Include(p => p.Practice)
                .Include(p => p.ProjectManager)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (p is null) return NotFound();
            return Ok(p);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Project project)
        {
            _db.Projects.Add(project);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = project.Id }, project);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Project project)
        {
            var existing = await _db.Projects.FindAsync(id);
            if (existing is null) return NotFound();

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
            var existing = await _db.Projects.FindAsync(id);
            if (existing is null) return NotFound();
            existing.IsDeleted = true;
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
