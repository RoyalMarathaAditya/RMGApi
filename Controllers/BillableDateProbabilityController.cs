using HRMS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Api.Controllers
{
    [ApiController]
    [Route("api/billable-date-probabilities")]
    public class BillableDateProbabilityController : ControllerBase
    {
        private readonly IBillableDateProbabilityService _service;

        public BillableDateProbabilityController(IBillableDateProbabilityService service)
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
