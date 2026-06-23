namespace HRMS.Api.DTOs.ProjectAllocationDtos
{
    public class AllocationDto
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeCode { get; set; } = string.Empty;
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string EmployeeProjectStatus { get; set; } = string.Empty;
        public string AllocationStatus { get; set; } = string.Empty;
        public DateTime AllocationStartDate { get; set; }
        public DateTime? AllocationEndDate { get; set; }
        public decimal AllocationPercentage { get; set; }
        public decimal? BillablePercentage { get; set; }
        public bool IsBillable { get; set; }
        public bool IsUtilized { get; set; }
        public DateTime? ProbableBillableDate { get; set; }
        public DateTime? NextAssignmentDate { get; set; }
        public string? Remarks { get; set; }
    }
}
