using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace HRMS.Api.Middleware
{
    public class LoggerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggerMiddleware> _logger;

        public LoggerMiddleware(RequestDelegate next, ILogger<LoggerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                // Log request
                var request = context.Request;
                request.EnableBuffering();

                string requestBody = string.Empty;
                if (request.ContentLength > 0)
                {
                    request.Body.Position = 0;
                    using var reader = new StreamReader(request.Body, leaveOpen: true);
                    requestBody = await reader.ReadToEndAsync();
                    request.Body.Position = 0;
                }

                // Redact sensitive headers like Authorization
                var requestHeadersDict = request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
                if (requestHeadersDict.ContainsKey("Authorization"))
                {
                    requestHeadersDict["Authorization"] = "REDACTED";
                }

                _logger.LogInformation("Incoming HTTP request {Method} {Path} QueryString={QueryString} Headers={Headers} Body={Body}",
                    request.Method, request.Path, request.QueryString, JsonSerializer.Serialize(requestHeadersDict), requestBody);

                // Capture the response
                var originalBodyStream = context.Response.Body;
                await using var responseBody = new MemoryStream();
                context.Response.Body = responseBody;

                await _next(context);

                // Read response
                context.Response.Body.Seek(0, SeekOrigin.Begin);
                string responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
                context.Response.Body.Seek(0, SeekOrigin.Begin);

                stopwatch.Stop();

                // Log response at appropriate level depending on status code
                if (context.Response.StatusCode >= 500)
                {
                    _logger.LogError("Outgoing HTTP response {StatusCode} ElapsedMs={ElapsedMs} Body={Body}",
                        context.Response.StatusCode, stopwatch.ElapsedMilliseconds, responseText);
                }
                else if (context.Response.StatusCode >= 400)
                {
                    _logger.LogWarning("Outgoing HTTP response {StatusCode} ElapsedMs={ElapsedMs} Body={Body}",
                        context.Response.StatusCode, stopwatch.ElapsedMilliseconds, responseText);
                }
                else
                {
                    _logger.LogInformation("Outgoing HTTP response {StatusCode} ElapsedMs={ElapsedMs} Body={Body}",
                        context.Response.StatusCode, stopwatch.ElapsedMilliseconds, responseText);
                }

                // Warn on slow requests (>2s)
                if (stopwatch.ElapsedMilliseconds > 2000)
                {
                    _logger.LogWarning("Slow request detected {Method} {Path} ElapsedMs={ElapsedMs} StatusCode={StatusCode}",
                        context.Request.Method, context.Request.Path, stopwatch.ElapsedMilliseconds, context.Response.StatusCode);
                }

                // Copy the contents of the new memory stream (which contains the response) to the original stream.
                context.Response.Body.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Unhandled exception while processing request {Method} {Path} ElapsedMs={ElapsedMs}",
                    context.Request.Method, context.Request.Path, stopwatch.ElapsedMilliseconds);
                throw;
            }
        }
    }
}
