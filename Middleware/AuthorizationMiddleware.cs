using System.Text.Json;
using System.Security.Claims;

namespace HRMS.Api.Middleware
{
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
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
    }
}
