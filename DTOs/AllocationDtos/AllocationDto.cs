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
        public string? Notes { get; set; }
    }

    public class UpdateAllocationDto
    {
        public int? ProjectId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? AllocationPercentage { get; set; }
        public string? AllocationStatus { get; set; }
        public string? Notes { get; set; }
    }
}
