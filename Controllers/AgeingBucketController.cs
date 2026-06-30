using HRMS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HRMS.Api.Controllers
{
    [ApiController]
    [Route("api/ageing-buckets")]
    public class AgeingBucketController : ControllerBase
    {
        private readonly IAgeingBucketService _service;
        private readonly ILogger<AgeingBucketController> _logger;

        public AgeingBucketController(IAgeingBucketService service, ILogger<AgeingBucketController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActive(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching ageing buckets...");
            var response = await _service.GetAllActiveAsync(cancellationToken);
            return Ok(response);
        }
    }
}
