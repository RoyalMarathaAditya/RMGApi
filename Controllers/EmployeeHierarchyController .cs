using HRMS.Api.DTOs.EmployeeDtos;
using HRMS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeHierarchyController : ControllerBase
    {
        private readonly IEmployeeHierarchyService _service;

        public EmployeeHierarchyController(IEmployeeHierarchyService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateEmployeeHierarchyDto request)
        {
            var result = await _service.CreateAsync(request);
            return Ok(result);
        }
    }
}