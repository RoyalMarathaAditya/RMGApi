using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using HRMS.Api.DTOs.Common;
using HRMS.Api.DTOs.UserDtos;
using HRMS.Api.Services.Interfaces.UserManagement;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Api.Controllers
{
    [ApiController]
    [Route("api/users")]
    [CustomAuthorize]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
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
            var sw = Stopwatch.StartNew();
            var result = await _userService.GetUsersAsync(pagination, cancellationToken);
            sw.Stop();

            _logger.LogInformation("GET /api/users [{ElapsedMs}ms] page={Page} size={Size} total={Total}",
                sw.ElapsedMilliseconds, pagination.PageNumber, pagination.PageSize, result.TotalCount);

            if (sw.ElapsedMilliseconds > 300)
                _logger.LogWarning("Slow GET /api/users [{ElapsedMs}ms] page={Page}", sw.ElapsedMilliseconds, pagination.PageNumber);

            Response.Headers["X-Response-Time-Ms"] = sw.ElapsedMilliseconds.ToString();
            Response.Headers["X-Total-Count"] = result.TotalCount.ToString();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id, CancellationToken cancellationToken)
        {
            var sw = Stopwatch.StartNew();
            var user = await _userService.GetUserByIdAsync(id, cancellationToken);
            sw.Stop();

            _logger.LogInformation("GET /api/users/{Id} [{ElapsedMs}ms]", id, sw.ElapsedMilliseconds);

            if (user is null)
                return NotFound(new { success = false, message = "User not found." });

            Response.Headers["X-Response-Time-Ms"] = sw.ElapsedMilliseconds.ToString();
            return Ok(user);
        }

        [HttpGet("available-employees")]
        public async Task<IActionResult> GetAvailableEmployees(CancellationToken cancellationToken)
        {
            var sw = Stopwatch.StartNew();
            var employees = await _userService.GetAvailableEmployeesAsync(cancellationToken);
            sw.Stop();

            _logger.LogInformation("GET /api/users/available-employees [{ElapsedMs}ms] count={Count}",
                sw.ElapsedMilliseconds, employees.Count);

            return Ok(employees.Select(e => new { e.Id, e.FullName, e.EmployeeCode, e.Email }));
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto, CancellationToken cancellationToken)
        {
            var sw = Stopwatch.StartNew();
            var result = await _userService.CreateUserAsync(dto, GetCurrentUser(), cancellationToken);
            sw.Stop();

            _logger.LogInformation("POST /api/users [{ElapsedMs}ms] success={Success}", sw.ElapsedMilliseconds, result.Success);

            if (!result.Success)
                return BadRequest(result);
            return CreatedAtAction(nameof(GetUser), new { id = result.Data?.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto dto, CancellationToken cancellationToken)
        {
            var sw = Stopwatch.StartNew();
            var result = await _userService.UpdateUserAsync(id, dto, GetCurrentUser(), cancellationToken);
            sw.Stop();

            _logger.LogInformation("PUT /api/users/{Id} [{ElapsedMs}ms] success={Success}", id, sw.ElapsedMilliseconds, result.Success);

            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id, CancellationToken cancellationToken)
        {
            var sw = Stopwatch.StartNew();
            var result = await _userService.DeleteUserAsync(id, cancellationToken);
            sw.Stop();

            _logger.LogInformation("DELETE /api/users/{Id} [{ElapsedMs}ms] success={Success}", id, sw.ElapsedMilliseconds, result.Success);

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

        [HttpPost("{id}/reset-password-default")]
        public async Task<IActionResult> ResetPasswordToDefault(int id, CancellationToken cancellationToken)
        {
            var result = await _userService.ResetPasswordToDefaultAsync(id, GetCurrentUser(), cancellationToken);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
