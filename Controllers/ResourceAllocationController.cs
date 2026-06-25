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

        public ResourceAllocationController(IResourceAllocationService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await _service.GetAllAsync(cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var result = await _service.GetByIdAsync(id, cancellationToken);
            if (result is null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAllocationDto dto, CancellationToken cancellationToken)
        {
            try
            {
                var userName = User.Identity?.Name ?? "system";
                var result = await _service.CreateAsync(dto, userName, cancellationToken);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateAllocationDto dto, CancellationToken cancellationToken)
        {
            try
            {
                var userName = User.Identity?.Name ?? "system";
                var result = await _service.UpdateAsync(id, dto, userName, cancellationToken);
                if (result is null) return NotFound();
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var result = await _service.DeleteAsync(id, cancellationToken);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpGet("{id}/history")]
        public async Task<IActionResult> GetHistory(int id, CancellationToken cancellationToken)
        {
            var result = await _service.GetHistoryAsync(id, cancellationToken);
            return Ok(result);
        }

        [HttpGet("calendar")]
        public async Task<IActionResult> GetCalendarData(CancellationToken cancellationToken)
        {
            var result = await _service.GetCalendarDataAsync(cancellationToken);
            return Ok(result);
        }

        [HttpGet("timeline")]
        public async Task<IActionResult> GetTimelineData(CancellationToken cancellationToken)
        {
            var result = await _service.GetTimelineDataAsync(cancellationToken);
            return Ok(result);
        }

        [HttpGet("employee/{employeeId}")]
        public async Task<IActionResult> GetEmployeeAllocations(int employeeId, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _service.GetEmployeeAllocationsAsync(employeeId, cancellationToken);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost("project")]
        public async Task<IActionResult> AddProjectAllocation(AddProjectAllocationDto dto, CancellationToken cancellationToken)
        {
            try
            {
                var userName = User.Identity?.Name ?? "system";
                var result = await _service.AddProjectAllocationAsync(dto, userName, cancellationToken);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("project/{allocationId}")]
        public async Task<IActionResult> UpdateProjectAllocation(int allocationId, UpdateProjectAllocationDto dto, CancellationToken cancellationToken)
        {
            try
            {
                var userName = User.Identity?.Name ?? "system";
                var result = await _service.UpdateProjectAllocationAsync(allocationId, dto, userName, cancellationToken);
                if (result is null) return NotFound();
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("project/{allocationId}")]
        public async Task<IActionResult> DeleteProjectAllocation(int allocationId, CancellationToken cancellationToken)
        {
            var result = await _service.DeleteProjectAllocationAsync(allocationId, cancellationToken);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpGet("employee/{employeeId}/capacity-summary")]
        public async Task<IActionResult> GetEmployeeCapacitySummary(int employeeId, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _service.GetEmployeeCapacitySummaryAsync(employeeId, cancellationToken);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("~/api/resourceallocation/employee-details/{employeeId}")]
        public async Task<IActionResult> GetEmployeeDetails(int employeeId, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _service.GetEmployeeDetailsAsync(employeeId, cancellationToken);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
