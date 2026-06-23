using HRMS.Api.DTOs;
using HRMS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Api.Controllers
{
    [ApiController]
    [Route("api/designations")]
    public class DesignationController : ControllerBase
    {
        private readonly IDesignationService _designationService;

        public DesignationController(IDesignationService designationService)
        {
            _designationService = designationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var response = await _designationService.GetAllAsync(cancellationToken);
            return Ok(response);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActive(CancellationToken cancellationToken)
        {
            var response = await _designationService.GetActiveAsync(cancellationToken);
            return Ok(response);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var response = await _designationService.GetByIdAsync(id, cancellationToken);
            if (!response.Success)
                return NotFound(response);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDesignationDto request, CancellationToken cancellationToken)
        {
            var response = await _designationService.CreateAsync(request, cancellationToken);
            if (!response.Success)
                return BadRequest(response);
            return CreatedAtAction(nameof(GetById), new { id = response.Data?.Id }, response);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDesignationDto request, CancellationToken cancellationToken)
        {
            var response = await _designationService.UpdateAsync(id, request, cancellationToken);
            if (!response.Success)
                return BadRequest(response);
            return Ok(response);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var response = await _designationService.DeleteAsync(id, cancellationToken);
            if (!response.Success)
                return NotFound(response);
            return Ok(response);
        }
    }
}