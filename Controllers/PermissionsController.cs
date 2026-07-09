using HRMS.Api.DTOs.UserDtos;
using HRMS.Api.Services.Interfaces.UserManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Api.Controllers
{
    [ApiController]
    [Route("api/permissions")]
    [Authorize]
    public class PermissionsController : ControllerBase
    {
        private readonly IPermissionService _permissionService;
        private readonly ILogger<PermissionsController> _logger;

        public PermissionsController(IPermissionService permissionService, ILogger<PermissionsController> logger)
        {
            _permissionService = permissionService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching permissions...");
            var permissions = await _permissionService.GetAllPermissionsAsync(cancellationToken);
            return Ok(permissions);
        }

        [HttpGet("by-role/{roleName}")]
        public async Task<IActionResult> GetByRole(string roleName, CancellationToken cancellationToken)
        {
            var permissions = await _permissionService.GetPermissionsByRoleAsync(roleName, cancellationToken);
            return Ok(permissions);
        }

        [HttpPut("role-permissions")]
        public async Task<IActionResult> SaveRolePermissions([FromBody] RolePermissionDto dto, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Saving permissions for role {Role}...", dto.RoleName);
            var result = await _permissionService.SaveRolePermissionsAsync(dto, cancellationToken);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }
    }
}