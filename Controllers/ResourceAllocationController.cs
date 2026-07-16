using System.Diagnostics;
using HRMS.Api.DTOs.AllocationDtos;
using HRMS.Api.Services.Interfaces.RMG;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Api.Controllers
{
    [ApiController]
    [Route("api/resource-allocations")]
    public class ResourceAllocationController : ControllerBase
    {
        private readonly IResourceAllocationService _service;
        private readonly ILogger<ResourceAllocationController> _logger;

        public ResourceAllocationController(IResourceAllocationService service, ILogger<ResourceAllocationController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching all resource allocations");
            var sw = Stopwatch.StartNew();
            var result = await _service.GetAllAsync(cancellationToken);
            sw.Stop();
            _logger.LogInformation("Fetched {Count} allocations in {ElapsedMs}ms", result.Count(), sw.ElapsedMilliseconds);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching resource allocation by Id={AllocationId}", id);
            var result = await _service.GetByIdAsync(id, cancellationToken);
            if (result is null)
            {
                _logger.LogWarning("Resource allocation {AllocationId} not found", id);
                return NotFound();
            }
            _logger.LogInformation("Resource allocation {AllocationId} retrieved", id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAllocationDto dto, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating resource allocation for EmployeeId={EmployeeId} ProjectId={ProjectId}",
                dto.EmployeeId, dto.ProjectId);
            try
            {
                var userName = User.Identity?.Name ?? "system";
                var sw = Stopwatch.StartNew();
                var result = await _service.CreateAsync(dto, userName, cancellationToken);
                sw.Stop();
                _logger.LogInformation("Resource allocation {AllocationId} created in {ElapsedMs}ms", result.Id, sw.ElapsedMilliseconds);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Resource allocation creation failed: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateAllocationDto dto, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating resource allocation {AllocationId}", id);
            try
            {
                var userName = User.Identity?.Name ?? "system";
                var sw = Stopwatch.StartNew();
                var result = await _service.UpdateAsync(id, dto, userName, cancellationToken);
                sw.Stop();
                if (result is null)
                {
                    _logger.LogWarning("Resource allocation {AllocationId} not found for update", id);
                    return NotFound();
                }
                _logger.LogInformation("Resource allocation {AllocationId} updated in {ElapsedMs}ms", id, sw.ElapsedMilliseconds);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Resource allocation {AllocationId} update failed: {Message}", id, ex.Message);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting resource allocation {AllocationId}", id);
            var sw = Stopwatch.StartNew();
            var result = await _service.DeleteAsync(id, cancellationToken);
            sw.Stop();
            if (!result)
            {
                _logger.LogWarning("Resource allocation {AllocationId} not found for deletion", id);
                return NotFound();
            }
            _logger.LogInformation("Resource allocation {AllocationId} deleted in {ElapsedMs}ms", id, sw.ElapsedMilliseconds);
            return NoContent();
        }

        [HttpGet("{id}/history")]
        public async Task<IActionResult> GetHistory(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching history for allocation {AllocationId}", id);
            var result = await _service.GetHistoryAsync(id, cancellationToken);
            return Ok(result);
        }

        [HttpGet("calendar")]
        public async Task<IActionResult> GetCalendarData(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching calendar data");
            var result = await _service.GetCalendarDataAsync(cancellationToken);
            return Ok(result);
        }

        [HttpGet("timeline")]
        public async Task<IActionResult> GetTimelineData(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching timeline data");
            var result = await _service.GetTimelineDataAsync(cancellationToken);
            return Ok(result);
        }

        [HttpGet("employee/{employeeId}")]
        public async Task<IActionResult> GetEmployeeAllocations(int employeeId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching allocations for employee {EmployeeId}", employeeId);
            try
            {
                var sw = Stopwatch.StartNew();
                var result = await _service.GetEmployeeAllocationsAsync(employeeId, cancellationToken);
                sw.Stop();
                _logger.LogInformation("Employee {EmployeeId} allocations retrieved in {ElapsedMs}ms with {Count} allocations",
                    employeeId, sw.ElapsedMilliseconds, result.Allocations.Count);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Employee {EmployeeId} not found: {Message}", employeeId, ex.Message);
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost("project")]
        public async Task<IActionResult> AddProjectAllocation(AddProjectAllocationDto dto, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Adding project allocation for EmployeeId={EmployeeId} ProjectId={ProjectId}",
                dto.EmployeeId, dto.ProjectId);
            try
            {
                var userName = User.Identity?.Name ?? "system";
                var sw = Stopwatch.StartNew();
                var result = await _service.AddProjectAllocationAsync(dto, userName, cancellationToken);
                sw.Stop();
                _logger.LogInformation("Project allocation {AllocationId} added in {ElapsedMs}ms", result.Id, sw.ElapsedMilliseconds);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Project allocation creation failed: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("bulk-project")]
        public async Task<IActionResult> AddBulkProjectAllocation(BulkProjectAllocationDto dto, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Bulk adding project allocations for {Count} employees, ProjectId={ProjectId}",
                dto.EmployeeIds.Count, dto.ProjectId);
            try
            {
                var userName = User.Identity?.Name ?? "system";
                var sw = Stopwatch.StartNew();
                var results = await _service.AddBulkProjectAllocationAsync(dto, userName, cancellationToken);
                sw.Stop();
                _logger.LogInformation("Bulk added {Count} project allocations in {ElapsedMs}ms", results.Count, sw.ElapsedMilliseconds);
                return CreatedAtAction(nameof(GetById), new { id = 0 }, results);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Bulk project allocation failed: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("employee/{employeeId}/details")]
        public async Task<IActionResult> UpdateEmployeeDetails(int employeeId, UpdateEmployeeDetailsDto dto, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating employee details for employee {EmployeeId}", employeeId);
            try
            {
                var sw = Stopwatch.StartNew();
                var result = await _service.UpdateEmployeeDetailsAsync(employeeId, dto, cancellationToken);
                sw.Stop();
                if (!result)
                {
                    _logger.LogWarning("Employee {EmployeeId} not found for details update", employeeId);
                    return NotFound(new { message = "Employee not found." });
                }
                _logger.LogInformation("Employee {EmployeeId} details updated in {ElapsedMs}ms", employeeId, sw.ElapsedMilliseconds);
                return Ok(new { message = "Employee details updated successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Employee {EmployeeId} details update failed: {Message}", employeeId, ex.Message);
                return BadRequest(new { message = ex.Message, innerException = ex.InnerException?.Message });
            }
        }

        [HttpPut("project/{allocationId}")]
        public async Task<IActionResult> UpdateProjectAllocation(int allocationId, UpdateProjectAllocationDto dto, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating project allocation {AllocationId}", allocationId);
            try
            {
                var userName = User.Identity?.Name ?? "system";
                var sw = Stopwatch.StartNew();
                var result = await _service.UpdateProjectAllocationAsync(allocationId, dto, userName, cancellationToken);
                sw.Stop();
                if (result is null)
                {
                    _logger.LogWarning("Project allocation {AllocationId} not found for update", allocationId);
                    return NotFound();
                }
                _logger.LogInformation("Project allocation {AllocationId} updated in {ElapsedMs}ms", allocationId, sw.ElapsedMilliseconds);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Project allocation {AllocationId} update failed: {Message}", allocationId, ex.Message);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("project/{allocationId}")]
        public async Task<IActionResult> DeleteProjectAllocation(int allocationId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting project allocation {AllocationId}", allocationId);
            var sw = Stopwatch.StartNew();
            var result = await _service.DeleteProjectAllocationAsync(allocationId, cancellationToken);
            sw.Stop();
            if (!result)
            {
                _logger.LogWarning("Project allocation {AllocationId} not found for deletion", allocationId);
                return NotFound();
            }
            _logger.LogInformation("Project allocation {AllocationId} deleted in {ElapsedMs}ms", allocationId, sw.ElapsedMilliseconds);
            return NoContent();
        }

        [HttpGet("employee/{employeeId}/capacity-summary")]
        public async Task<IActionResult> GetEmployeeCapacitySummary(int employeeId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching capacity summary for employee {EmployeeId}", employeeId);
            try
            {
                var sw = Stopwatch.StartNew();
                var result = await _service.GetEmployeeCapacitySummaryAsync(employeeId, cancellationToken);
                sw.Stop();
                _logger.LogInformation("Employee {EmployeeId} capacity summary retrieved in {ElapsedMs}ms", employeeId, sw.ElapsedMilliseconds);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Employee {EmployeeId} not found for capacity summary: {Message}", employeeId, ex.Message);
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("~/api/resourceallocation/employee-details/{employeeId}")]
        public async Task<IActionResult> GetEmployeeDetails(int employeeId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching employee details for employee {EmployeeId}", employeeId);
            try
            {
                var sw = Stopwatch.StartNew();
                var result = await _service.GetEmployeeDetailsAsync(employeeId, cancellationToken);
                sw.Stop();
                _logger.LogInformation("Employee {EmployeeId} details retrieved in {ElapsedMs}ms", employeeId, sw.ElapsedMilliseconds);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Employee {EmployeeId} not found for details: {Message}", employeeId, ex.Message);
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
