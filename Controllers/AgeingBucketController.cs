using HRMS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Api.Controllers
{
    [ApiController]
    [Route("api/ageing-buckets")]
    public class AgeingBucketController : ControllerBase
    {
        private readonly IAgeingBucketService _service;

        public AgeingBucketController(IAgeingBucketService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActive(CancellationToken cancellationToken)
        {
            var response = await _service.GetAllActiveAsync(cancellationToken);
            return Ok(response);
        }
    }
}
