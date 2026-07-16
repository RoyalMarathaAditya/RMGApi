using HRMS.Api.DTOs.RmgDashboardDtos;
using HRMS.Api.Services;
using HRMS.Api.Services.Interfaces.RMG;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HRMS.Api.Controllers
{
    [ApiController]
    [Route("api/rmg-dashboard")]
    public class RmgDashboardController : ControllerBase
    {
        private readonly IRmgDashboardService _service;
        private readonly IExcelExportService _excelExportService;
        private readonly ILogger<RmgDashboardController> _logger;

        public RmgDashboardController(IRmgDashboardService service, IExcelExportService excelExportService, ILogger<RmgDashboardController> logger)
        {
            _service = service;
            _excelExportService = excelExportService;
            _logger = logger;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching RMG dashboard summary...");
            var result = await _service.GetSummaryAsync(cancellationToken);
            return Ok(result);
        }

        [HttpGet("grid")]
        public async Task<IActionResult> GetGridData([FromQuery] DashboardFilterDto? filter, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching RMG dashboard grid data...");
            var result = await _service.GetGridDataAsync(filter, cancellationToken);
            return Ok(result);
        }

        [HttpGet("suitable-resources/{projectId}")]
        public async Task<IActionResult> GetSuitableResources(int projectId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching suitable resources for project {ProjectId}...", projectId);
            var result = await _service.GetSuitableResourcesAsync(projectId, cancellationToken);
            return Ok(result);
        }

        [HttpGet("resource-availability")]
        public async Task<IActionResult> GetResourceAvailability(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching resource availability...");
            var result = await _service.GetResourceAvailabilityAsync(cancellationToken);
            return Ok(result);
        }

        [HttpGet("practice-utilization")]
        public async Task<IActionResult> GetPracticeUtilization(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching practice utilization...");
            var result = await _service.GetPracticeUtilizationAsync(cancellationToken);
            return Ok(result);
        }

        [HttpGet("export")]
        public async Task<IActionResult> ExportGrid([FromQuery] DashboardFilterDto? filter, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Exporting detailed resource allocations to Excel...");
            var bytes = await _excelExportService.ExportDetailedResourceAllocationsAsync(
                filter?.SearchTerm, filter?.Practice, filter?.ResourceStatus, cancellationToken);
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"ResourceAllocation_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx");
        }
    }
}
