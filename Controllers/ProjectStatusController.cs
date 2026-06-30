using HRMS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HRMS.Api.Controllers
{
    [ApiController]
    [Route("api/project-status")]
    public class ProjectStatusController : ControllerBase
    {
        private readonly IProjectStatusService _projectStatusService;
        private readonly ILogger<ProjectStatusController> _logger;

        public ProjectStatusController(IProjectStatusService projectStatusService, ILogger<ProjectStatusController> logger)
        {
            _projectStatusService = projectStatusService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActive(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching project statuses...");
            var response = await _projectStatusService.GetAllActiveAsync(cancellationToken);
            return Ok(response);
        }
    }
}
