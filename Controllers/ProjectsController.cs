using AutoMapper;
using HRMS.Api.DTOs;
using HRMS.Api.Models;
using HRMS.Api.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HRMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectRepository _projectRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<ProjectsController> _logger;

        public ProjectsController(IProjectRepository projectRepo, IMapper mapper, ILogger<ProjectsController> logger)
        {
            _projectRepo = projectRepo;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching projects...");
            var items = await _projectRepo.GetAllAsync(cancellationToken);
            var dtos = _mapper.Map<IEnumerable<ProjectDto>>(items);
            return Ok(dtos);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActive(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching active projects...");
            var items = await _projectRepo.GetActiveProjectsAsync(cancellationToken);
            var dtos = _mapper.Map<IEnumerable<ProjectDto>>(items);
            return Ok(dtos);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching project with {Id}...", id);
            var project = await _projectRepo.GetByIdAsync(id, cancellationToken);
            if (project is null)
            {
                _logger.LogWarning("Project {Id} not found", id);
                return NotFound();
            }
            var dto = _mapper.Map<ProjectDto>(project);
            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProjectDto dto, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating project...");
            var project = _mapper.Map<Project>(dto);
            var created = await _projectRepo.CreateAsync(project, cancellationToken);
            var resultDto = _mapper.Map<ProjectDto>(created);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, resultDto);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProjectDto dto, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating project {Id}...", id);
            var existing = await _projectRepo.GetByIdAsync(id, cancellationToken);
            if (existing is null)
            {
                _logger.LogWarning("Project {Id} not found for update", id);
                return NotFound();
            }

            _mapper.Map(dto, existing);
            existing.ModifiedOn = DateTime.UtcNow;

            var updated = await _projectRepo.UpdateAsync(existing, cancellationToken);
            var resultDto = _mapper.Map<ProjectDto>(updated);
            return Ok(resultDto);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting project {Id}...", id);
            var existing = await _projectRepo.GetByIdAsync(id, cancellationToken);
            if (existing is null)
            {
                _logger.LogWarning("Project {Id} not found for deletion", id);
                return NotFound();
            }
            await _projectRepo.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
