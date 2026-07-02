using HRMS.Api.DTOs;
using HRMS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Api.Controllers
{
    [ApiController]
    [Route("api/column-value-mappings")]
    public class ColumnValueMappingsController : ControllerBase
    {
        private readonly IColumnValueMappingService _service;
        private readonly ILogger<ColumnValueMappingsController> _logger;

        public ColumnValueMappingsController(IColumnValueMappingService service, ILogger<ColumnValueMappingsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            _logger.LogInformation("Fetching all column value mappings...");
            var result = await _service.GetAllAsync(ct);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            _logger.LogInformation("Fetching column value mapping {Id}...", id);
            var result = await _service.GetByIdAsync(id, ct);
            if (!result.Success)
                return NotFound(result);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateColumnValueMappingDto dto, CancellationToken ct)
        {
            _logger.LogInformation("Creating column value mapping...");
            var result = await _service.CreateAsync(dto, ct);
            if (!result.Success)
                return BadRequest(result);
            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateColumnValueMappingDto dto, CancellationToken ct)
        {
            _logger.LogInformation("Updating column value mapping {Id}...", id);
            var result = await _service.UpdateAsync(id, dto, ct);
            if (!result.Success)
                return NotFound(result);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            _logger.LogInformation("Deleting column value mapping {Id}...", id);
            var result = await _service.DeleteAsync(id, ct);
            if (!result.Success)
                return NotFound(result);
            return Ok(result);
        }
    }
}
