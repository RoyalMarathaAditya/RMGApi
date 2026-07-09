using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using HRMS.Api.DTOs.Common;
using HRMS.Api.DTOs.UserDtos;
using HRMS.Api.Services.Interfaces.UserManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Api.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserManagementService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserManagementService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        private string? GetCurrentUser() =>
            User.FindFirst(ClaimTypes.Name)?.Value
            ?? User.FindFirst(JwtRegisteredClaimNames.Email)?.Value;

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] PaginationParams pagination, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching users...");
            var result = await _userService.GetUsersAsync(pagination, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id, CancellationToken cancellationToken)
        {
            var user = await _userService.GetUserByIdAsync(id, cancellationToken);
            if (user is null)
                return NotFound(new { success = false, message = "User not found." });
            return Ok(user);
        }

        [HttpGet("available-employees")]
        public async Task<IActionResult> GetAvailableEmployees(CancellationToken cancellationToken)
        {
            var employees = await _userService.GetAvailableEmployeesAsync(cancellationToken);
            return Ok(employees.Select(e => new { e.Id, e.FullName, e.EmployeeCode }));
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating user...");
            var result = await _userService.CreateUserAsync(dto, GetCurrentUser(), cancellationToken);
            if (!result.Success)
                return BadRequest(result);
            return CreatedAtAction(nameof(GetUser), new { id = result.Data?.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto dto, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating user {Id}...", id);
            var result = await _userService.UpdateUserAsync(id, dto, GetCurrentUser(), cancellationToken);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting user {Id}...", id);
            var result = await _userService.DeleteUserAsync(id, cancellationToken);
            if (!result.Success)
                return NotFound(result);
            return Ok(result);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto, CancellationToken cancellationToken)
        {
            var result = await _userService.ResetPasswordAsync(dto, cancellationToken);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("{id}/lock")]
        public async Task<IActionResult> LockUser(int id, CancellationToken cancellationToken)
        {
            var result = await _userService.LockUserAsync(id, GetCurrentUser(), cancellationToken);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("{id}/unlock")]
        public async Task<IActionResult> UnlockUser(int id, CancellationToken cancellationToken)
        {
            var result = await _userService.UnlockUserAsync(id, cancellationToken);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("{id}/activate")]
        public async Task<IActionResult> ActivateUser(int id, CancellationToken cancellationToken)
        {
            var result = await _userService.ActivateUserAsync(id, GetCurrentUser(), cancellationToken);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("{id}/deactivate")]
        public async Task<IActionResult> DeactivateUser(int id, CancellationToken cancellationToken)
        {
            var result = await _userService.DeactivateUserAsync(id, GetCurrentUser(), cancellationToken);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }
    }
}