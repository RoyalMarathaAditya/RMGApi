using HRMS.Api.Data;
using HRMS.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ClientsController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
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
            var c = await _db.Clients
                .AsNoTracking()
                .Include(c => c.ClientStatus)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (c is null) return NotFound();
            return Ok(c);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Client client)
        {
            _db.Clients.Add(client);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = client.Id }, client);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Client client)
        {
            var existing = await _db.Clients.FindAsync(id);
            if (existing is null) return NotFound();

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
            var existing = await _db.Clients.FindAsync(id);
            if (existing is null) return NotFound();
            existing.IsDeleted = true;
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
