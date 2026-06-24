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

        [HttpGet("practice-summary")]
        public async Task<IActionResult> GetPracticeSummary(CancellationToken cancellationToken)
        {
            var result = await _service.GetPracticeSummaryAsync(cancellationToken);
            return Ok(result);
        }
    }
}
