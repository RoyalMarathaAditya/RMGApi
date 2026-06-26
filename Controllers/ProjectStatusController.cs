using HRMS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Api.Controllers
{
    [ApiController]
    [Route("api/project-status")]
    public class ProjectStatusController : ControllerBase
    {
        private readonly IProjectStatusService _projectStatusService;

        public ProjectStatusController(IProjectStatusService projectStatusService)
        {
            _projectStatusService = projectStatusService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActive(CancellationToken cancellationToken)
        {
            var response = await _projectStatusService.GetAllActiveAsync(cancellationToken);
            return Ok(response);
        }
    }
}
