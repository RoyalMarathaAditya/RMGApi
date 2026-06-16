using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

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
                var path = httpContext.Request.Path.Value ?? "";

                // 1. High-speed bypass for Swagger files, static assets, and login routes
                if (path == "/" ||
                    path.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase) ||
                    path.StartsWith("/openapi", StringComparison.OrdinalIgnoreCase) ||
                    path.StartsWith("/api/auth/login", StringComparison.OrdinalIgnoreCase) ||
                    path.EndsWith(".js", StringComparison.OrdinalIgnoreCase) ||
                    path.EndsWith(".css", StringComparison.OrdinalIgnoreCase) ||
                    path.EndsWith(".html", StringComparison.OrdinalIgnoreCase))
                {
                    await _next(httpContext);
                    return;
                }

                // 2. Extract Authorization header keys
                if (!httpContext.Request.Headers.TryGetValue("Authorization", out var authHeaderValues))
                {
                    httpContext.Response.ContentType = "application/json";
                    httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await httpContext.Response.WriteAsJsonAsync(new { message = "Missing Authorization header." });
                    return;
                }

                var authHeader = authHeaderValues.ToString().Trim();
                if (string.IsNullOrWhiteSpace(authHeader))
                {
                    httpContext.Response.ContentType = "application/json";
                    httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await httpContext.Response.WriteAsJsonAsync(new { message = "Empty Authorization header value." });
                    return;
                }

                // 3. String extraction and sanitisation
                string token = authHeader;
                if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    token = authHeader.Substring("Bearer ".Length).Trim();
                }

                // Strip accidental duplicate headers like "Bearer Bearer token"
                if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    token = token.Substring("Bearer ".Length).Trim();
                }

                // FIX: TRUNCATE TRAILING JSON / WHITESPACE DATA
                // If there's a space followed by trailing JSON elements (like your space at index 406), 
                // this cleanly splits the string and grabs ONLY the first contiguous block (the valid JWT)
                if (token.Contains(" "))
                {
                    token = token.Split(' ')[0];
                }

                // Trim leading/trailing garbage characters, quotes, or commas left behind by JSON objects
                token = token.Trim(' ', '"', '\'', '\t', '\r', '\n', ',', '{', '}');

                if (token.Contains("%"))
                {
                    token = Uri.UnescapeDataString(token);
                }

                // 4. CRITICAL STRUCTURAL GUARD: Pre-verify isolated JWT format before parsing
                // Now that trailing content is stripped, the remaining isolated token string 
                // MUST contain exactly 2 dots. If it doesn't, reject it immediately.
                if (string.IsNullOrEmpty(token) || token.Count(c => c == '.') != 2)
                {
                    httpContext.Response.ContentType = "application/json";
                    httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await httpContext.Response.WriteAsJsonAsync(new
                    {
                        message = "Authentication failed: The provided token is not a well-formed JWT string.",
                        detail = "Expected standard Compact format (Header.Payload.Signature) with no trailing data."
                    });
                    return;
                }

                try
                {
                    // 5. Native token extraction mapping using classic, error-free handler
                    var handler = new JwtSecurityTokenHandler();
                    var jwt = handler.ReadJwtToken(token);

                    // 6. Enforce token expiration validation checks
                    if (jwt.ValidTo < DateTime.UtcNow)
                    {
                        httpContext.Response.ContentType = "application/json";
                        httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await httpContext.Response.WriteAsJsonAsync(new { message = "Authentication failed: Access token has expired." });
                        return;
                    }

                    // 7. Normalize standard claim shortcuts to official .NET schemas
                    var claims = jwt.Claims.Select(c =>
                    {
                        if (c.Type == "role" || c.Type == "roles" || c.Type == ClaimTypes.Role)
                        {
                            return new Claim(ClaimTypes.Role, c.Value);
                        }

                        if (c.Type == "unique_name" || c.Type == "sub" || c.Type == "name" || c.Type == ClaimTypes.Name)
                        {
                            return new Claim(ClaimTypes.Name, c.Value);
                        }

                        return new Claim(c.Type, c.Value);
                    }).ToList();

                    // 8. Construct user principal and flag request as Authenticated
                    var identity = new ClaimsIdentity(claims, "Bearer", ClaimTypes.Name, ClaimTypes.Role);
                    var principal = new ClaimsPrincipal(identity);

                    httpContext.User = principal;
                }
                catch (Exception tokenEx)
                {
                    httpContext.Response.ContentType = "application/json";
                    httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await httpContext.Response.WriteAsJsonAsync(new
                    {
                        message = "Authentication failed: The token profile data could not be parsed.",
                        detail = tokenEx.Message
                    });
                    return;
                }

                // If authentication passes, proceed downstream to your CustomAuthorizationMiddleware
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                if (!httpContext.Response.HasStarted)
                {
                    httpContext.Response.ContentType = "application/json";
                    httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await httpContext.Response.WriteAsJsonAsync(new { message = "Authentication middleware system failure exception.", detail = ex.Message });
                }
            }
        }
    }
}
