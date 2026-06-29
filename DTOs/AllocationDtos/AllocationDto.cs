namespace HRMS.Api.DTOs.AllocationDtos
{
    public class AllocationDto
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeCode { get; set; } = string.Empty;
        public string? Designation { get; set; }
        public string Practice { get; set; } = string.Empty;
        public string? PracticeHead { get; set; }
        public string? Skills { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal AllocationPercentage { get; set; }
        public string AllocationStatus { get; set; } = string.Empty;
        public string? AllocationType { get; set; }
        public string? BillableStatus { get; set; }
        public string? Notes { get; set; }
        public decimal TotalAllocated { get; set; }
        public decimal AvailableCapacity { get; set; }
        public string ResourceStatus { get; set; } = string.Empty;
    }

    public class CreateAllocationDto
    {
        public int EmployeeId { get; set; }
        public int ProjectId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal AllocationPercentage { get; set; }
        public string? AllocationStatus { get; set; }
        public string? AllocationType { get; set; }
        public string? BillableStatus { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateAllocationDto
    {
        public int? ProjectId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? AllocationPercentage { get; set; }
        public string? AllocationStatus { get; set; }
        public string? AllocationType { get; set; }
        public string? BillableStatus { get; set; }
        public string? Notes { get; set; }
    }

    public class ProjectAllocationDto
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public int? ClientId { get; set; }
        public string? ClientName { get; set; }
        public Guid? ProjectStatusId { get; set; }
        public Guid? StatusId { get; set; }
        public Guid? ProbableNextAssignmentId { get; set; }
        public DateTime? ProbableNextAssignmentDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal AllocationPercentage { get; set; }
        public string? BillableStatus { get; set; }
        public string? AllocationType { get; set; }
        public string AllocationStatus { get; set; } = string.Empty;
    }

    public class AddProjectAllocationDto
    {
        public int EmployeeId { get; set; }
        public int ProjectId { get; set; }
        public int? ClientId { get; set; }
        public Guid? ProjectStatusId { get; set; }
        public Guid? StatusId { get; set; }
        public Guid? ProbableNextAssignmentId { get; set; }
        public DateTime? ProbableNextAssignmentDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal AllocationPercentage { get; set; }
        public string? BillableStatus { get; set; }
        public string? AllocationType { get; set; }
        public string? AllocationStatus { get; set; }
    }

    public class UpdateProjectAllocationDto
    {
        public int? ProjectId { get; set; }
        public int? ClientId { get; set; }
        public Guid? ProjectStatusId { get; set; }
        public Guid? StatusId { get; set; }
        public Guid? ProbableNextAssignmentId { get; set; }
        public DateTime? ProbableNextAssignmentDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? AllocationPercentage { get; set; }
        public string? BillableStatus { get; set; }
        public string? AllocationType { get; set; }
        public string? AllocationStatus { get; set; }
    }

    public class EmployeeAllocationDto
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeCode { get; set; } = string.Empty;
        public string? Designation { get; set; }
        public string Practice { get; set; } = string.Empty;
        public string? Skills { get; set; }
        public string? PrimarySkill { get; set; }
        public decimal? TotalExperience { get; set; }
        public decimal CurrentUtilization { get; set; }
        public decimal AvailableCapacity { get; set; }
        public string ResourceStatus { get; set; } = string.Empty;
        public List<ProjectAllocationDto> Allocations { get; set; } = new();
    }

    public class EmployeeCapacitySummaryDto
    {
        public decimal TotalCapacity { get; set; } = 100;
        public decimal AllocatedCapacity { get; set; }
        public decimal AvailableCapacity { get; set; }
        public string ResourceStatus { get; set; } = string.Empty;
    }
}
