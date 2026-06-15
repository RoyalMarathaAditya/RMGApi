using HRMS.Api.DTOs;
using HRMS.Api.Models;
using HRMS.Api.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly IClientRepository _clientRepo;

        public ClientsController(IClientRepository clientRepo)
        {
            _clientRepo = clientRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var clients = await _clientRepo.GetAllAsync();
            return Ok(clients.Select(c => new ClientDto
            {
                Id = c.Id,
                Name = c.Name,
                ContactEmail = c.ContactEmail,
                ContactNumber = c.ContactNumber,
                Address = c.Address
            }));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var c = await _clientRepo.GetByIdAsync(id);
            if (c is null) return NotFound();
            return Ok(new ClientDto
            {
                Id = c.Id,
                Name = c.Name,
                ContactEmail = c.ContactEmail,
                ContactNumber = c.ContactNumber,
                Address = c.Address
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ClientDto dto)
        {
            var client = new Client
            {
                Name = dto.Name,
                ContactEmail = dto.ContactEmail,
                ContactNumber = dto.ContactNumber,
                Address = dto.Address,
            };
            var created = await _clientRepo.CreateAsync(client);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ClientDto dto)
        {
            var existing = await _clientRepo.GetByIdAsync(id);
            if (existing is null) return NotFound();
            existing.Name = dto.Name;
            existing.ContactEmail = dto.ContactEmail;
            existing.ContactNumber = dto.ContactNumber;
            existing.Address = dto.Address;
            var updated = await _clientRepo.UpdateAsync(existing);
            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _clientRepo.DeleteAsync(id);
            return NoContent();
        }
    }
}
