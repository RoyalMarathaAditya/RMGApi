using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace HRMS.Api.Middleware
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {

                if (!httpContext.Request.Headers.TryGetValue("Authorization", out var authHeaderValues))
                {
                    httpContext.Response.ContentType = "application/json";
                    httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    var response = new
                    {
                        message = "Missing Authorization header.",
                    };
                    await httpContext.Response.WriteAsync(JsonSerializer.Serialize(response));
                    return;
                }

                var authHeader = authHeaderValues.ToString();
                if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    httpContext.Response.ContentType = "application/json";
                    httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    var response = new
                    {
                        message = "Invalid Authorization header format. Expected 'Bearer <token>'.",
                    };
                    await httpContext.Response.WriteAsync(JsonSerializer.Serialize(response));
                    return;
                }

                var token = authHeader.Substring("Bearer ".Length).Trim();
                if (string.IsNullOrEmpty(token))
                {
                    httpContext.Response.ContentType = "application/json";
                    httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    var response = new
                    {
                        message = "Empty bearer token.",
                    };
                    await httpContext.Response.WriteAsync(JsonSerializer.Serialize(response));
                    return;
                }

                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    // Parse the token without validating signature here. This middleware's purpose is to populate HttpContext.User
                    // for downstream authorization checks. Real signature validation should be done by the framework or a dedicated validator.
                    var jwt = handler.ReadJwtToken(token);
                    var claims = jwt.Claims.Select(c => new Claim(c.Type, c.Value));
                    var identity = new ClaimsIdentity(claims, "Bearer");
                    var principal = new ClaimsPrincipal(identity);
                    httpContext.User = principal;
                }
                catch (Exception ex)
                {
                    httpContext.Response.ContentType = "application/json";
                    httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    var response = new
                    {
                        message = "Invalid JWT token.",
                        detail = ex.Message
                    };
                    await httpContext.Response.WriteAsync(JsonSerializer.Serialize(response));
                    return;
                }

                await _next(httpContext);
            }
            catch (Exception ex)
            {
                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                var response = new
                {
                    message = "Authentication middleware failure.",
                    detail = ex.Message
                };
                await httpContext.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }
}
