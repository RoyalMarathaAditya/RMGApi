using HRMS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Api.Controllers
{
    [ApiController]
    [Route("api/current-billing-statuses")]
    public class CurrentBillingStatusController : ControllerBase
    {
        private readonly ICurrentBillingStatusService _service;

        public CurrentBillingStatusController(ICurrentBillingStatusService service)
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
