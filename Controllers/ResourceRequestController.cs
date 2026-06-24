using HRMS.Api.DTOs.ResourceRequestDtos;
using HRMS.Api.Services.Interfaces.RMG;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Api.Controllers
{
    [ApiController]
    [Route("api/resource-requests")]
    public class ResourceRequestController : ControllerBase
    {
        private readonly IResourceRequestService _service;

        public ResourceRequestController(IResourceRequestService service)
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

        [HttpPost]
        public async Task<IActionResult> Create(CreateResourceRequestDto dto, CancellationToken cancellationToken)
        {
            var requestedById = 1;
            var result = await _service.CreateAsync(dto, requestedById, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, UpdateResourceRequestDto dto, CancellationToken cancellationToken)
        {
            var userName = User.Identity?.Name ?? "system";
            var result = await _service.UpdateStatusAsync(id, dto.Status ?? "", dto.Notes, userName, cancellationToken);
            if (result is null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var result = await _service.DeleteAsync(id, cancellationToken);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
