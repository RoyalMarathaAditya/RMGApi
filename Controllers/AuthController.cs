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
                {
                    return BadRequest(new { message = "Email and password are required." });
                }

                var response = await _authService.AuthenticateAsync(request);
                if (response is null)
                {
                    return Unauthorized(new { message = "Invalid credentials." });
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for {Email}", request?.Email);
                // Return a safe error message to the client
                return Problem(detail: "An unexpected error occurred while attempting to authenticate.", statusCode: 500);
            }
        }
    }
}
