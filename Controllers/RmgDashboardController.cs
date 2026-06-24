using HRMS.Api.DTOs.RmgDashboardDtos;
using HRMS.Api.Services.Interfaces.RMG;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Api.Controllers
{
    [ApiController]
    [Route("api/rmg-dashboard")]
    public class RmgDashboardController : ControllerBase
    {
        private readonly IRmgDashboardService _service;

        public RmgDashboardController(IRmgDashboardService service)
        {
            _service = service;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary(CancellationToken cancellationToken)
        {
            var result = await _service.GetSummaryAsync(cancellationToken);
            return Ok(result);
        }

        [HttpGet("grid")]
        public async Task<IActionResult> GetGridData([FromQuery] DashboardFilterDto? filter, CancellationToken cancellationToken)
        {
            var result = await _service.GetGridDataAsync(filter, cancellationToken);
            return Ok(result);
        }

        [HttpGet("suitable-resources/{projectId}")]
        public async Task<IActionResult> GetSuitableResources(int projectId, CancellationToken cancellationToken)
        {
            var result = await _service.GetSuitableResourcesAsync(projectId, cancellationToken);
            return Ok(result);
        }

        [HttpGet("resource-availability")]
        public async Task<IActionResult> GetResourceAvailability(CancellationToken cancellationToken)
        {
            var result = await _service.GetResourceAvailabilityAsync(cancellationToken);
            return Ok(result);
        }

        [HttpGet("practice-utilization")]
        public async Task<IActionResult> GetPracticeUtilization(CancellationToken cancellationToken)
        {
            var result = await _service.GetPracticeUtilizationAsync(cancellationToken);
            return Ok(result);
        }
    }
}
