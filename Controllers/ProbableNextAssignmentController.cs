using HRMS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HRMS.Api.Controllers
{
    [ApiController]
    [Route("api/probable-next-assignments")]
    public class ProbableNextAssignmentController : ControllerBase
    {
        private readonly IProbableNextAssignmentService _service;
        private readonly ILogger<ProbableNextAssignmentController> _logger;

        public ProbableNextAssignmentController(IProbableNextAssignmentService service, ILogger<ProbableNextAssignmentController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActive(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching probable next assignments...");
            var response = await _service.GetAllActiveAsync(cancellationToken);
            return Ok(response);
        }
    }
}
