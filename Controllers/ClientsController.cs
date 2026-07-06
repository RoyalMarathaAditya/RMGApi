using HRMS.Api.DTOs;
using HRMS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly IClientService _clientService;
        private readonly ILogger<ClientsController> _logger;

        public ClientsController(IClientService clientService, ILogger<ClientsController> logger)
        {
            _clientService = clientService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching all clients...");
            var result = await _clientService.GetAllAsync(cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching client with {Id}...", id);
            var result = await _clientService.GetByIdAsync(id, cancellationToken);
            if (!result.Success)
                return NotFound(result);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateClientDto request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating client...");
            var userName = User.Identity?.Name ?? "System";
            var result = await _clientService.CreateAsync(request, userName, cancellationToken);
            if (!result.Success)
                return BadRequest(result);
            return CreatedAtAction(nameof(GetById), new { id = ((ClientDto)result.Data!).Id }, result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateClientDto request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating client {Id}...", id);
            var userName = User.Identity?.Name ?? "System";
            var result = await _clientService.UpdateAsync(id, request, userName, cancellationToken);
            if (!result.Success)
                return result.Message.Contains("not found") ? NotFound(result) : BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting client {Id}...", id);
            var userName = User.Identity?.Name ?? "System";
            var result = await _clientService.DeleteAsync(id, userName, cancellationToken);
            if (!result.Success)
                return NotFound(result);
            return Ok(result);
        }
    }
}
