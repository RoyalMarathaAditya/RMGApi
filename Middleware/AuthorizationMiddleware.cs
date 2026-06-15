using System.Text.Json;
using System.Security.Claims;

namespace HRMS.Api.Middleware
{
    public class AuthorizationMiddleware
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

        public AuthorizationMiddleware(RequestDelegate next)
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

                // If no authenticated user, return 401
                if (httpContext.User?.Identity == null || !httpContext.User.Identity.IsAuthenticated)
                {
                    httpContext.Response.ContentType = "application/json";
                    httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    var response = new
                    {
                        message = "Unauthenticated. Access denied.",
                    };
                    await httpContext.Response.WriteAsync(JsonSerializer.Serialize(response));
                    return;
                }

                // Example: enforce that user must have at least one of roles specified on endpoint via Items["AllowedRoles"]
                if (httpContext.Items.TryGetValue("AllowedRoles", out var allowedObj) && allowedObj is string[] allowedRoles && allowedRoles.Length > 0)
                {
                    var userRoles = httpContext.User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToArray();
                    if (!userRoles.Any(r => allowedRoles.Contains(r)))
                    {
                        httpContext.Response.ContentType = "application/json";
                        httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
                        var response = new
                        {
                            message = "Forbidden. User does not have required role.",
                        };
                        await httpContext.Response.WriteAsync(JsonSerializer.Serialize(response));
                        return;
                    }
                }

                await _next(httpContext);
            }
            catch (Exception ex)
            {
                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                var response = new
                {
                    message = "Authorization middleware failure.",
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
