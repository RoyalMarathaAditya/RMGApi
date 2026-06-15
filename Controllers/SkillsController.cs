using HRMS.Api.DTOs;
using HRMS.Api.Models;
using HRMS.Api.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SkillsController : ControllerBase
    {
        private readonly ISkillRepository _skillRepo;

        public SkillsController(ISkillRepository skillRepo)
        {
            _skillRepo = skillRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _skillRepo.GetAllAsync();
            return Ok(items.Select(s => new SkillDto { Id = s.Id, Name = s.Name, Description = s.Description }));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var s = await _skillRepo.GetByIdAsync(id);
            if (s is null) return NotFound();
            return Ok(new SkillDto { Id = s.Id, Name = s.Name, Description = s.Description });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SkillDto dto)
        {
            var s = new Skill { Name = dto.Name, Description = dto.Description };
            var created = await _skillRepo.CreateAsync(s);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] SkillDto dto)
        {
            var existing = await _skillRepo.GetByIdAsync(id);
            if (existing is null) return NotFound();
            existing.Name = dto.Name;
            existing.Description = dto.Description;
            var updated = await _skillRepo.UpdateAsync(existing);
            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _skillRepo.DeleteAsync(id);
            return NoContent();
        }
    }
}
