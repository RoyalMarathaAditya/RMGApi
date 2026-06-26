using HRMS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Api.Controllers
{
    [ApiController]
    [Route("api/probable-next-assignments")]
    public class ProbableNextAssignmentController : ControllerBase
    {
        private readonly IProbableNextAssignmentService _service;

        public ProbableNextAssignmentController(IProbableNextAssignmentService service)
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
