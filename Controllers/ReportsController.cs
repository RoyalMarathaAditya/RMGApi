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
        public async Task<IActionResult> GetPracticeWiseReport(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching practice wise report...");
            var result = await _reportService.GetPracticeWiseReportAsync(cancellationToken);
            return Ok(result);
        }

        [HttpGet("practice-wise/export")]
        public async Task<IActionResult> ExportPracticeWiseReport(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Exporting practice wise report...");
            var bytes = await _reportService.ExportPracticeWiseReportAsync(cancellationToken);
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"PracticeWiseReport_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx");
        }
    }
}
