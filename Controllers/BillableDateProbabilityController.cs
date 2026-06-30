using HRMS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HRMS.Api.Controllers
{
    [ApiController]
    [Route("api/billable-date-probabilities")]
    public class BillableDateProbabilityController : ControllerBase
    {
        private readonly IBillableDateProbabilityService _service;
        private readonly ILogger<BillableDateProbabilityController> _logger;

        public BillableDateProbabilityController(IBillableDateProbabilityService service, ILogger<BillableDateProbabilityController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActive(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching billable date probabilities...");
            var response = await _service.GetAllActiveAsync(cancellationToken);
            return Ok(response);
        }
    }
}
