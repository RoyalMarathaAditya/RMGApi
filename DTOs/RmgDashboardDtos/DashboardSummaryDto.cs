namespace HRMS.Api.DTOs.RmgDashboardDtos
{
    public class DashboardSummaryDto
    {
        public int TotalEmployees { get; set; }
        public int TotalPractices { get; set; }
        public int AvailableResources { get; set; }
        public int AllocatedResources { get; set; }
        public int FullyAllocatedResources { get; set; }
        public int OverallocatedResources { get; set; }
        public int BenchResources { get; set; }
        public int ResourcesOnLeave { get; set; }
        public decimal PracticeUtilizationPercentage { get; set; }
    }

    public class DashboardGridDto
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeCode { get; set; } = string.Empty;
        public string? Designation { get; set; }
        public string? Department { get; set; }
        public string Practice { get; set; } = string.Empty;
        public string? PracticeHead { get; set; }
        public string? Skills { get; set; }
        public string? CurrentProject { get; set; }
        public decimal AllocationPercentage { get; set; }
        public decimal AvailableCapacity { get; set; }
        public string ResourceStatus { get; set; } = string.Empty;
        public string? AllocationStatus { get; set; }
    }

    public class DashboardFilterDto
    {
        public string? SearchTerm { get; set; }
        public string? EmployeeName { get; set; }
        public string? EmployeeCode { get; set; }
        public string? Department { get; set; }
        public string? Practice { get; set; }
        public string? PracticeHead { get; set; }
        public string? Skill { get; set; }
        public string? Designation { get; set; }
        public string? Project { get; set; }
        public string? AllocationStatus { get; set; }
        public string? ResourceStatus { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}
