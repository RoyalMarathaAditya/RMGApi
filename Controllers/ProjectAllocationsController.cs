using HRMS.Api.DTOs.ProjectAllocationDtos;
using HRMS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Api.Controllers
{
    [ApiController]
    [Route("api/projectallocations")]
    public class ProjectAllocationsController : ControllerBase
    {
        private readonly IProjectAllocationService _service;

        public ProjectAllocationsController(IProjectAllocationService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await _service.GetAllAsync(cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var result = await _service.GetByIdAsync(id, cancellationToken);
            if (result is null) return NotFound();
            return Ok(result);
        }

        [HttpGet("byemployee/{employeeId}")]
        public async Task<IActionResult> GetByEmployee(int employeeId, CancellationToken cancellationToken)
        {
            var result = await _service.GetByEmployeeIdAsync(employeeId, cancellationToken);
            return Ok(result);
        }

        [HttpGet("byproject/{projectId}")]
        public async Task<IActionResult> GetByProject(int projectId, CancellationToken cancellationToken)
        {
            var result = await _service.GetByProjectIdAsync(projectId, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAllocationDto dto, CancellationToken cancellationToken)
        {
            var result = await _service.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateAllocationDto dto, CancellationToken cancellationToken)
        {
            var result = await _service.UpdateAsync(id, dto, cancellationToken);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(id, cancellationToken);
            return NoContent();
        }

        [HttpPost("{id}/release")]
        public async Task<IActionResult> ReleaseResource(int id, CancellationToken cancellationToken)
        {
            await _service.ReleaseResourceAsync(id, cancellationToken);
            return Ok(new { message = "Resource released successfully" });
        }
    }
}
