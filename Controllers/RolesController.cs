using HRMS.Api.DTOs.UserDtos;
using HRMS.Api.Services.Interfaces.UserManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Api.Controllers
{
    [ApiController]
    [Route("api/roles")]
    [Authorize]
    public class RolesController : ControllerBase
    {
        private readonly IRoleManagementService _roleService;
        private readonly ILogger<RolesController> _logger;

        public RolesController(IRoleManagementService roleService, ILogger<RolesController> logger)
        {
            _roleService = roleService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetRoles(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching roles...");
            var roles = await _roleService.GetRolesAsync(cancellationToken);
            return Ok(roles);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetRole(Guid id, CancellationToken cancellationToken)
        {
            var role = await _roleService.GetRoleByIdAsync(id, cancellationToken);
            if (role is null)
                return NotFound(new { success = false, message = "Role not found." });
            return Ok(role);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto dto, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating role...");
            var result = await _roleService.CreateRoleAsync(dto, cancellationToken);
            if (!result.Success)
                return BadRequest(result);
            return CreatedAtAction(nameof(GetRole), new { id = result.Data?.Id }, result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateRole(Guid id, [FromBody] UpdateRoleDto dto, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating role {Id}...", id);
            var result = await _roleService.UpdateRoleAsync(id, dto, cancellationToken);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteRole(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting role {Id}...", id);
            var result = await _roleService.DeleteRoleAsync(id, cancellationToken);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("{id:guid}/activate")]
        public async Task<IActionResult> ActivateRole(Guid id, CancellationToken cancellationToken)
        {
            var result = await _roleService.ActivateRoleAsync(id, cancellationToken);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("{id:guid}/deactivate")]
        public async Task<IActionResult> DeactivateRole(Guid id, CancellationToken cancellationToken)
        {
            var result = await _roleService.DeactivateRoleAsync(id, cancellationToken);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }
    }
}