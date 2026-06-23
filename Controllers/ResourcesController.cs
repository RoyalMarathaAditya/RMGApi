using HRMS.Api.Services;
using HRMS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Api.Controllers
{
    [ApiController]
    [Route("api/resources")]
    public class ResourcesController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public ResourcesController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("availability")]
        public async Task<IActionResult> GetAvailability(CancellationToken cancellationToken)
        {
            var result = await _dashboardService.GetResourceAvailabilityAsync(cancellationToken);
            return Ok(result);
        }

        [HttpGet("bench")]
        public async Task<IActionResult> GetBench(CancellationToken cancellationToken)
        {
            var result = await _dashboardService.GetBenchResourcesAsync(cancellationToken);
            return Ok(result);
        }
    }
}
