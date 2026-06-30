using HRMS.Api.DTOs;
using HRMS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HRMS.Api.Controllers
{
    [ApiController]
    [Route("api/designations")]
    public class DesignationController : ControllerBase
    {
        private readonly IDesignationService _designationService;
        private readonly ILogger<DesignationController> _logger;

        public DesignationController(IDesignationService designationService, ILogger<DesignationController> logger)
        {
            _designationService = designationService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching designations...");
            var response = await _designationService.GetAllAsync(cancellationToken);
            return Ok(response);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActive(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching active designations...");
            var response = await _designationService.GetActiveAsync(cancellationToken);
            return Ok(response);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching designation with {Id}...", id);
            var response = await _designationService.GetByIdAsync(id, cancellationToken);
            if (!response.Success)
            {
                _logger.LogWarning("Designation {Id} not found", id);
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDesignationDto request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating designation...");
            var response = await _designationService.CreateAsync(request, cancellationToken);
            if (!response.Success)
                return BadRequest(response);
            return CreatedAtAction(nameof(GetById), new { id = response.Data?.Id }, response);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDesignationDto request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating designation {Id}...", id);
            var response = await _designationService.UpdateAsync(id, request, cancellationToken);
            if (!response.Success)
                return BadRequest(response);
            return Ok(response);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting designation {Id}...", id);
            var response = await _designationService.DeleteAsync(id, cancellationToken);
            if (!response.Success)
            {
                _logger.LogWarning("Designation {Id} not found for deletion", id);
                return NotFound(response);
            }
            return Ok(response);
        }
    }
}