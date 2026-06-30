using HRMS.Api.DTOs.PIPDtos;
using HRMS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HRMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PIPsController : ControllerBase
    {
        private readonly IPipService _service;
        private readonly ILogger<PIPsController> _logger;

        public PIPsController(IPipService service, ILogger<PIPsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching PIPs...");
            var result = await _service.GetAllAsync(cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching PIP with {Id}...", id);
            var result = await _service.GetByIdAsync(id, cancellationToken);
            if (result is null)
            {
                _logger.LogWarning("PIP {Id} not found", id);
                return NotFound();
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePipDto dto, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating PIP...");
            var result = await _service.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting PIP {Id}...", id);
            await _service.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
