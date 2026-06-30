using HRMS.Api.DTOs.ResourceRequestDtos;
using HRMS.Api.Services.Interfaces.RMG;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HRMS.Api.Controllers
{
    [ApiController]
    [Route("api/resource-requests")]
    public class ResourceRequestController : ControllerBase
    {
        private readonly IResourceRequestService _service;
        private readonly ILogger<ResourceRequestController> _logger;

        public ResourceRequestController(IResourceRequestService service, ILogger<ResourceRequestController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching resource requests...");
            var result = await _service.GetAllAsync(cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching resource request with {Id}...", id);
            var result = await _service.GetByIdAsync(id, cancellationToken);
            if (result is null)
            {
                _logger.LogWarning("Resource request {Id} not found", id);
                return NotFound();
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateResourceRequestDto dto, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating resource request...");
            var requestedById = 1;
            var result = await _service.CreateAsync(dto, requestedById, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, UpdateResourceRequestDto dto, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating resource request {Id} status...", id);
            var userName = User.Identity?.Name ?? "system";
            var result = await _service.UpdateStatusAsync(id, dto.Status ?? "", dto.Notes, userName, cancellationToken);
            if (result is null)
            {
                _logger.LogWarning("Resource request {Id} not found for status update", id);
                return NotFound();
            }
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting resource request {Id}...", id);
            var result = await _service.DeleteAsync(id, cancellationToken);
            if (!result)
            {
                _logger.LogWarning("Resource request {Id} not found for deletion", id);
                return NotFound();
            }
            return NoContent();
        }
    }
}
