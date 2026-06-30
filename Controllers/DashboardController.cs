using HRMS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HRMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _service;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(IDashboardService service, ILogger<DashboardController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet("practice-summary")]
        public async Task<IActionResult> GetPracticeSummary(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching dashboard practice summary...");
            var result = await _service.GetPracticeSummaryAsync(cancellationToken);
            return Ok(result);
        }
    }
}
