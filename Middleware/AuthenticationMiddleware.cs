using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace HRMS.Api.Middleware
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string[] _excludedPaths = new[]
        {
            "/swagger",
            "/swagger-ui.html",
            "/swagger-ui.js",
            "/swagger-ui.css",
            "/swagger-ui-bundle.js",
            "/swagger-ui-standalone-preset.js",
            "/swagger-ui-standalone-preset.css",
            "/swagger-initializer.js",
            "/openapi",
            "/api/auth/login",
            "/",
            "/index.html"
        };

        public AuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                var path = httpContext.Request.Path.Value ?? "";

                // Check if the request path is in the excluded list
                if (IsPathExcluded(path))
                {
                    await _next(httpContext);
                    return;
                }

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

        private bool IsPathExcluded(string path)
        {
            // Normalize path for comparison
            var normalizedPath = path.TrimEnd('/').ToLowerInvariant();

            // Check for exact matches
            foreach (var excludedPath in _excludedPaths)
            {
                if (normalizedPath.Equals(excludedPath.TrimEnd('/').ToLowerInvariant(), StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            // Check if path starts with any excluded prefix (for nested resources like /swagger/*)
            foreach (var excludedPath in _excludedPaths)
            {
                var prefix = excludedPath.TrimEnd('/').ToLowerInvariant();
                if (normalizedPath.StartsWith(prefix + "/", StringComparison.OrdinalIgnoreCase) ||
                    normalizedPath.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
