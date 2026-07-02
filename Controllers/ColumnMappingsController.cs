using HRMS.Api.DTOs;
using HRMS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Api.Controllers
{
    [ApiController]
    [Route("api/column-mappings")]
    public class ColumnMappingsController : ControllerBase
    {
        private readonly IColumnMappingService _service;
        private readonly ILogger<ColumnMappingsController> _logger;

        public ColumnMappingsController(IColumnMappingService service, ILogger<ColumnMappingsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            _logger.LogInformation("Fetching all column mappings...");
            var result = await _service.GetAllAsync(ct);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            _logger.LogInformation("Fetching column mapping {Id}...", id);
            var result = await _service.GetByIdAsync(id, ct);
            if (!result.Success)
                return NotFound(result);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateColumnMappingDto dto, CancellationToken ct)
        {
            _logger.LogInformation("Creating column mapping...");
            var result = await _service.CreateAsync(dto, ct);
            if (!result.Success)
                return BadRequest(result);
            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateColumnMappingDto dto, CancellationToken ct)
        {
            _logger.LogInformation("Updating column mapping {Id}...", id);
            var result = await _service.UpdateAsync(id, dto, ct);
            if (!result.Success)
                return NotFound(result);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            _logger.LogInformation("Deleting column mapping {Id}...", id);
            var result = await _service.DeleteAsync(id, ct);
            if (!result.Success)
                return NotFound(result);
            return Ok(result);
        }
    }
}
