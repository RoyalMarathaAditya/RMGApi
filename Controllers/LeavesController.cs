using HRMS.Api.DTOs.LeaveDtos;
using HRMS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HRMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeavesController : ControllerBase
    {
        private readonly ILeaveService _service;
        private readonly ILogger<LeavesController> _logger;

        public LeavesController(ILeaveService service, ILogger<LeavesController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching leaves...");
            var result = await _service.GetAllAsync(cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching leave with {Id}...", id);
            var result = await _service.GetByIdAsync(id, cancellationToken);
            if (result is null)
            {
                _logger.LogWarning("Leave {Id} not found", id);
                return NotFound();
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateLeaveDto dto, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating leave...");
            var result = await _service.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting leave {Id}...", id);
            await _service.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
