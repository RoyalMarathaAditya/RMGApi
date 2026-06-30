using HRMS.Api.Data;
using HRMS.Api.DTOs.Common;
using HRMS.Api.DTOs.EmployeeDtos;
using HRMS.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HRMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly IBulkImportService _bulkImportService;
        private readonly AppDbContext _db;
        private readonly ILogger<EmployeesController> _logger;

        public EmployeesController(IEmployeeService employeeService, IBulkImportService bulkImportService, AppDbContext db, ILogger<EmployeesController> logger)
        {
            _employeeService = employeeService;
            _bulkImportService = bulkImportService;
            _db = db;
            _logger = logger;
        }

        [HttpGet("download-template")]
        public IActionResult DownloadTemplate()
        {
            _logger.LogInformation("Downloading employee import template...");
            var bytes = _bulkImportService.GenerateTemplate();
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "EmployeeImportTemplate.xlsx");
        }

        [HttpPost("bulk-upload")]
        [RequestSizeLimit(10 * 1024 * 1024)]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> BulkUpload(IFormFile file, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Bulk uploading employees...");
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No file uploaded." });

            var extension = Path.GetExtension(file.FileName).ToLower();
            if (extension != ".xlsx" && extension != ".xls")
                return BadRequest(new { message = "Only .xlsx and .xls files are accepted." });

            if (file.Length > 10 * 1024 * 1024)
                return BadRequest(new { message = "File size must not exceed 10 MB." });

            var uploadedBy = User.Identity?.Name;
            var result = await _bulkImportService.ImportAsync(file, uploadedBy, cancellationToken);
            return Ok(result);
        }

        [HttpGet("leaders")]
        public async Task<IActionResult> GetLeaders(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching employee leaders...");
            var leaders = await _db.Employees
                .Where(e => !e.IsDeleted)
                .OrderBy(e => e.FullName)
                .Select(e => new { e.Id, e.FullName })
                .ToListAsync(cancellationToken);
            return Ok(leaders);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching employees...");
            var result = await _employeeService.GetAllAsync(cancellationToken);
            return Ok(result);
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged([FromQuery] PaginationParams parameters, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching paged employees...");
            var result = await _employeeService.GetPagedAsync(parameters, cancellationToken);
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string q, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Searching employees with query {Query}...", q);
            var result = await _employeeService.SearchAsync(q, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching employee with {Id}...", id);
            var result = await _employeeService.GetByIdAsync(id, cancellationToken);
            if (!result.Success)
            {
                _logger.LogWarning("Employee {Id} not found", id);
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateEmployeeDto dto, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating employee...");
            var result = await _employeeService.CreateAsync(dto, cancellationToken);
            if (!result.Success)
                return BadRequest(result);
            return CreatedAtAction(nameof(GetById), new { id = ((EmployeeDto)result.Data!).Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateEmployeeDto dto, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating employee {Id}...", id);
            var result = await _employeeService.UpdateAsync(id, dto, cancellationToken);
            if (!result.Success)
            {
                _logger.LogWarning("Employee {Id} not found for update", id);
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting employee {Id}...", id);
            var result = await _employeeService.DeleteAsync(id, cancellationToken);
            if (!result.Success)
            {
                _logger.LogWarning("Employee {Id} not found for deletion", id);
                return NotFound(result);
            }
            return Ok(result);
        }
    }
}
