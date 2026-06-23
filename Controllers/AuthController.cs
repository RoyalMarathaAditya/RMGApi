using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using HRMS.Api.DTOs;
using HRMS.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (request is null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                    return BadRequest(new { message = "Email and password are required." });

                var response = await _authService.AuthenticateAsync(request);
                if (response is null)
                    return Unauthorized(new { message = "Invalid credentials." });

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for {Email}", request?.Email);
                return Problem(detail: "An unexpected error occurred while attempting to authenticate.", statusCode: 500);
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            try
            {
                var refreshToken = request?.RefreshToken;
                if (string.IsNullOrEmpty(refreshToken))
                {
                    refreshToken = Request.Cookies["RefreshToken"];
                    if (string.IsNullOrEmpty(refreshToken))
                        return Unauthorized(new { message = "No refresh token provided." });
                }

                var response = await _authService.RefreshTokenAsync(refreshToken);
                if (response is null)
                    return Unauthorized(new { message = "Invalid or expired refresh token." });

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing token");
                return Problem(detail: "An unexpected error occurred while refreshing the token.", statusCode: 500);
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest request)
        {
            try
            {
                var refreshToken = request?.RefreshToken;
                if (!string.IsNullOrEmpty(refreshToken))
                    await _authService.RevokeRefreshTokenAsync(refreshToken);

                return Ok(new { message = "Logged out successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return Problem(detail: "An unexpected error occurred during logout.", statusCode: 500);
            }
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                    return Unauthorized(new { message = "Invalid token claims." });

                var user = await _authService.GetCurrentUserAsync(userId);
                if (user is null)
                    return NotFound(new { message = "User not found." });

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching current user");
                return Problem(detail: "An unexpected error occurred.", statusCode: 500);
            }
        }
    }
}
