using System.Net;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Logging;

namespace HRMS.Api.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                // Log the exception with request context
                _logger.LogError(ex, "Unhandled exception while processing request {Method} {Path}", httpContext.Request.Method, httpContext.Request.Path);

                httpContext.Response.ContentType = "application/json";

                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                var generic = new
                {
                    message = "An unexpected error occurred.",
                    detail = ex.Message,
                };
                await httpContext.Response.WriteAsync(JsonSerializer.Serialize(generic));
            }
        }
    }
}
