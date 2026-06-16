using HRMS.Api;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

public class CustomAuthorizationMiddleware
{
    private readonly RequestDelegate _next;
    public CustomAuthorizationMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        // 1. Get the current endpoint from the routing pipeline
        var endpoint = context.GetEndpoint();
        if (endpoint == null)
        {
            await _next(context);
            return;
        }

        // 2. Check if the endpoint or controller has your [CustomAuthorize] attribute
        var authorizeAttribute = endpoint.Metadata.GetMetadata<CustomAuthorizeAttribute>();

        if (authorizeAttribute != null)
        {
            // 3. Block request immediately if the user is not authenticated by the CustomAuthMiddleware
            if (context.User?.Identity?.IsAuthenticated != true)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized; // 401 Unauthorized
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new { message = "Authentication required." });
                return;
            }

            // 4. If specific roles are mentioned (e.g., [CustomAuthorize("Admin,Manager")]), validate them
            if (!string.IsNullOrEmpty(authorizeAttribute.Roles))
            {
                var requiredRoles = authorizeAttribute.Roles
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(r => r.Trim());

                // Check if user has at least one matching role claim
                bool hasMatchingRole = requiredRoles.Any(role => context.User.IsInRole(role));

                if (!hasMatchingRole)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden; // 403 Forbidden
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(new { message = "You do not have permission to access this resource." });
                    return;
                }
            }
        }

        // If authorization passes, let the request proceed to the controller
        await _next(context);
    }
}
