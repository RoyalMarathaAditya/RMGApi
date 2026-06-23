using HRMS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _service;

        public DashboardController(IDashboardService service)
        {
            _service = service;
        }

        [HttpGet("resource-summary")]
        public async Task<IActionResult> GetResourceSummary(CancellationToken cancellationToken)
        {
            var result = await _service.GetResourceSummaryAsync(cancellationToken);
            return Ok(result);
        }

        [HttpGet("practice-summary")]
        public async Task<IActionResult> GetPracticeSummary(CancellationToken cancellationToken)
        {
            var result = await _service.GetPracticeSummaryAsync(cancellationToken);
            return Ok(result);
        }

        [HttpGet("allocation-summary")]
        public async Task<IActionResult> GetAllocationSummary(CancellationToken cancellationToken)
        {
            var result = await _service.GetAllocationSummaryAsync(cancellationToken);
            return Ok(result);
        }

        [HttpGet("upcoming-releases")]
        public async Task<IActionResult> GetUpcomingReleases(CancellationToken cancellationToken)
        {
            var result = await _service.GetUpcomingReleasesAsync(cancellationToken);
            return Ok(result);
        }

        [HttpGet("utilization")]
        public async Task<IActionResult> GetUtilization(CancellationToken cancellationToken)
        {
            var result = await _service.GetUtilizationPercentageAsync(cancellationToken);
            return Ok(new { utilizationPercentage = result });
        }
    }
}
