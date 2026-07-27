using HRMS.Api.Services.Interfaces.RMG;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Api.Controllers
{
    [ApiController]
    [Route("api/reports")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly ILogger<ReportsController> _logger;

        public ReportsController(IReportService reportService, ILogger<ReportsController> logger)
        {
            _reportService = reportService;
            _logger = logger;
        }

        [HttpGet("practice-wise")]
        public async Task<IActionResult> GetPracticeWiseReport([FromQuery] bool? engineeringOnly, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching practice wise report...");
            var result = await _reportService.GetPracticeWiseReportAsync(engineeringOnly ?? false, cancellationToken);
            return Ok(result);
        }

        [HttpGet("practice-wise/export")]
        public async Task<IActionResult> ExportPracticeWiseReport([FromQuery] bool? engineeringOnly, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Exporting practice wise report...");
            var bytes = await _reportService.ExportPracticeWiseReportAsync(engineeringOnly ?? false, cancellationToken);
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"PracticeWiseReport_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx");
        }

        [HttpGet("client-wise")]
        public async Task<IActionResult> GetClientWiseReport([FromQuery] string? clientIds, [FromQuery] string? status, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching client wise report...");
            var result = await _reportService.GetClientWiseReportAsync(clientIds, status, cancellationToken);
            return Ok(result);
        }

        [HttpGet("client-wise/charts")]
        public async Task<IActionResult> GetClientWiseChartData(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching client wise chart data...");
            var result = await _reportService.GetClientWiseChartDataAsync(cancellationToken);
            return Ok(result);
        }

        [HttpGet("project-wise")]
        public async Task<IActionResult> GetProjectWiseReport([FromQuery] string? client, [FromQuery] string? practice, [FromQuery] string? status, [FromQuery] string? search, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching project wise report...");
            var result = await _reportService.GetProjectWiseReportAsync(client, practice, status, search, cancellationToken);
            return Ok(result);
        }

        [HttpGet("project-wise/charts")]
        public async Task<IActionResult> GetProjectWiseChartData(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching project wise chart data...");
            var result = await _reportService.GetProjectWiseChartDataAsync(cancellationToken);
            return Ok(result);
        }
    }
}
