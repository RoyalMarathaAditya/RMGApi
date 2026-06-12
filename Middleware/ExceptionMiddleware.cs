using System.Net;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;

namespace HRMS.Api.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                httpContext.Response.ContentType = "application/json";

                if (ex is SecurityTokenException)
                {
                    httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    var response = new
                    {
                        message = "Authentication error.",
                        detail = ex.Message,
                    };
                    await httpContext.Response.WriteAsync(JsonSerializer.Serialize(response));
                    return;
                }

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
