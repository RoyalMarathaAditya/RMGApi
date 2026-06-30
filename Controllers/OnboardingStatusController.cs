using HRMS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HRMS.Api.Controllers
{
    [ApiController]
    [Route("api/onboarding-statuses")]
    public class OnboardingStatusController : ControllerBase
    {
        private readonly IOnboardingStatusService _service;
        private readonly ILogger<OnboardingStatusController> _logger;

        public OnboardingStatusController(IOnboardingStatusService service, ILogger<OnboardingStatusController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActive(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching onboarding statuses...");
            var response = await _service.GetAllActiveAsync(cancellationToken);
            return Ok(response);
        }
    }
}
