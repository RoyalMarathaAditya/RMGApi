using HRMS.Api.DTOs.Common;
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

        public RmgDashboardController(
            IRmgDashboardService service,
            IExcelExportService excelExportService,
            ILogger<RmgDashboardController> logger)
        {
            _service = service;
            _excelExportService = excelExportService;
            _logger = logger;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary(CancellationToken cancellationToken)
        {
            _logger.LogInformation("GET /api/rmg-dashboard/summary");
            var result = await _service.GetSummaryAsync(cancellationToken);
            return Ok(result);
        }

        [HttpGet("grid")]
        public async Task<IActionResult> GetGridData(
            CancellationToken cancellationToken,
            [FromQuery] DashboardFilterDto? filter = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 25,
            [FromQuery] string? sortField = null,
            [FromQuery] string? sortDirection = "asc")
        {
            _logger.LogInformation("GET /api/rmg-dashboard/grid page={Page} size={PageSize}", page, pageSize);
            var paging = new PagedFilterDto
            {
                Page = Math.Max(1, page),
                PageSize = Math.Clamp(pageSize, 1, 200),
                SortField = sortField,
                SortDirection = sortDirection
            };
            var result = await _service.GetGridDataAsync(filter, paging, cancellationToken);
            return Ok(result);
        }

        [HttpGet("suitable-resources/{projectId}")]
        public async Task<IActionResult> GetSuitableResources(int projectId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("GET /api/rmg-dashboard/suitable-resources/{ProjectId}", projectId);
            var result = await _service.GetSuitableResourcesAsync(projectId, cancellationToken);
            return Ok(result);
        }

        [HttpGet("resource-availability")]
        public async Task<IActionResult> GetResourceAvailability(CancellationToken cancellationToken)
        {
            _logger.LogInformation("GET /api/rmg-dashboard/resource-availability");
            var result = await _service.GetResourceAvailabilityAsync(cancellationToken);
            return Ok(result);
        }

        [HttpGet("practice-utilization")]
        public async Task<IActionResult> GetPracticeUtilization(CancellationToken cancellationToken)
        {
            _logger.LogInformation("GET /api/rmg-dashboard/practice-utilization");
            var result = await _service.GetPracticeUtilizationAsync(cancellationToken);
            return Ok(result);
        }

        [HttpGet("export")]
        public async Task<IActionResult> ExportGrid(
            [FromQuery] DashboardFilterDto? filter,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("GET /api/rmg-dashboard/export");
            var bytes = await _excelExportService.ExportDetailedResourceAllocationsAsync(
                filter?.SearchTerm, filter?.Practice, filter?.ResourceStatus, cancellationToken);
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"ResourceAllocation_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx");
        }

        [HttpPost("cache/invalidate")]
        public async Task<IActionResult> InvalidateCache()
        {
            _logger.LogInformation("POST /api/rmg-dashboard/cache/invalidate");
            await _service.InvalidateDashboardCacheAsync();
            return Ok(new { message = "Dashboard cache invalidated" });
        }
    }
}
