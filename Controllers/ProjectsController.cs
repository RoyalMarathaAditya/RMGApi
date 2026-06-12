using HRMS.Api.DTOs;
using HRMS.Api.Models;
using HRMS.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectDetailsRepository _projectRepo;

        public ProjectsController(IProjectDetailsRepository projectRepo)
        {
            _projectRepo = projectRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _projectRepo.GetAllAsync();
            return Ok(items.Select(p => new ProjectDetailsDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                ClientId = p.ClientId,
                ClientName = p.Client?.Name ?? string.Empty,
                LocationId = p.LocationId,
                LocationName = p.Location?.Name ?? string.Empty,
                IsActive = p.IsActive,
                Skills = p.ProjectSkills.Select(ps => new SkillDto { Id = ps.Skill?.Id ?? 0, Name = ps.Skill?.Name ?? string.Empty, Description = ps.Skill?.Description ?? string.Empty }).ToList()
            }));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var p = await _projectRepo.GetByIdAsync(id);
            if (p is null) return NotFound();
            return Ok(new ProjectDetailsDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                ClientId = p.ClientId,
                ClientName = p.Client?.Name ?? string.Empty,
                LocationId = p.LocationId,
                LocationName = p.Location?.Name ?? string.Empty,
                IsActive = p.IsActive,
                Skills = p.ProjectSkills.Select(ps => new SkillDto { Id = ps.Skill?.Id ?? 0, Name = ps.Skill?.Name ?? string.Empty, Description = ps.Skill?.Description ?? string.Empty }).ToList()
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProjectDetailsDto dto)
        {
            var project = new ProjectDetails
            {
                Name = dto.Name,
                Description = dto.Description,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                ClientId = dto.ClientId,
                LocationId = dto.LocationId,
                IsActive = dto.IsActive,
                ProjectSkills = dto.Skills.Select(s => new ProjectDetailsSkill { SkillId = s.Id, Level = "Intermediate" }).ToList()
            };
            var created = await _projectRepo.CreateAsync(project);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProjectDetailsDto dto)
        {
            var existing = await _projectRepo.GetByIdAsync(id);
            if (existing is null) return NotFound();
            existing.Name = dto.Name;
            existing.Description = dto.Description;
            existing.StartDate = dto.StartDate;
            existing.EndDate = dto.EndDate;
            existing.ClientId = dto.ClientId;
            existing.LocationId = dto.LocationId;
            existing.IsActive = dto.IsActive;
            existing.ProjectSkills = dto.Skills.Select(s => new ProjectDetailsSkill { SkillId = s.Id, Level = "Intermediate" }).ToList();
            var updated = await _projectRepo.UpdateAsync(existing);
            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _projectRepo.DeleteAsync(id);
            return NoContent();
        }
    }
}
