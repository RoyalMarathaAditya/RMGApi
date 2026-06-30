using HRMS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HRMS.Api.Controllers
{
    [ApiController]
    [Route("api/billing-buckets")]
    public class BillingBucketController : ControllerBase
    {
        private readonly IBillingBucketService _service;
        private readonly ILogger<BillingBucketController> _logger;

        public BillingBucketController(IBillingBucketService service, ILogger<BillingBucketController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActive(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching billing buckets...");
            var response = await _service.GetAllActiveAsync(cancellationToken);
            return Ok(response);
        }
    }
}
