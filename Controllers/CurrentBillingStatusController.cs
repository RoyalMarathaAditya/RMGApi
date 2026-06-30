using HRMS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HRMS.Api.Controllers
{
    [ApiController]
    [Route("api/current-billing-statuses")]
    public class CurrentBillingStatusController : ControllerBase
    {
        private readonly ICurrentBillingStatusService _service;
        private readonly ILogger<CurrentBillingStatusController> _logger;

        public CurrentBillingStatusController(ICurrentBillingStatusService service, ILogger<CurrentBillingStatusController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActive(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching current billing statuses...");
            var response = await _service.GetAllActiveAsync(cancellationToken);
            return Ok(response);
        }
    }
}
