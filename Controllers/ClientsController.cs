using HRMS.Api.Data;
using HRMS.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HRMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ILogger<ClientsController> _logger;

        public ClientsController(AppDbContext db, ILogger<ClientsController> logger)
        {
            _db = db;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Fetching clients...");
            var clients = await _db.Clients
                .AsNoTracking()
                .Include(c => c.ClientStatus)
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.ContractStartDate,
                    c.ContractEndDate,
                    StatusName = c.ClientStatus.Name,
                    c.Location
                })
                .ToListAsync();
            return Ok(clients);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation("Fetching client with {Id}...", id);
            var c = await _db.Clients
                .AsNoTracking()
                .Include(c => c.ClientStatus)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (c is null)
            {
                _logger.LogWarning("Client {Id} not found", id);
                return NotFound();
            }
            return Ok(c);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Client client)
        {
            _logger.LogInformation("Creating client...");
            _db.Clients.Add(client);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = client.Id }, client);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Client client)
        {
            _logger.LogInformation("Updating client {Id}...", id);
            var existing = await _db.Clients.FindAsync(id);
            if (existing is null)
            {
                _logger.LogWarning("Client {Id} not found for update", id);
                return NotFound();
            }

            existing.Name = client.Name;
            existing.ContractStartDate = client.ContractStartDate;
            existing.ContractEndDate = client.ContractEndDate;
            existing.StatusId = client.StatusId;
            existing.Location = client.Location;

            await _db.SaveChangesAsync();
            return Ok(existing);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Deleting client {Id}...", id);
            var existing = await _db.Clients.FindAsync(id);
            if (existing is null)
            {
                _logger.LogWarning("Client {Id} not found for deletion", id);
                return NotFound();
            }
            existing.IsDeleted = true;
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
