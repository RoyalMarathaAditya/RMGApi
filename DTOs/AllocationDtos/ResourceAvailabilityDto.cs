namespace HRMS.Api.DTOs.AllocationDtos
{
    public class ResourceAvailabilityDto
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeCode { get; set; } = string.Empty;
        public string? Designation { get; set; }
        public string Practice { get; set; } = string.Empty;
        public string? PracticeHead { get; set; }
        public string? Department { get; set; }
        public string? Skills { get; set; }
        public string? CurrentProject { get; set; }
        public decimal TotalAllocated { get; set; }
        public decimal AvailableCapacity { get; set; }
        public string ResourceStatus { get; set; } = string.Empty;
        public DateTime? NextAvailableDate { get; set; }
    }

    public class ResourceSuggestionDto
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public decimal SkillMatchPercentage { get; set; }
        public decimal AvailabilityPercentage { get; set; }
        public decimal TotalAllocated { get; set; }
        public decimal AvailableCapacity { get; set; }
        public string ResourceStatus { get; set; } = string.Empty;
    }

    public class PracticeUtilizationDto
    {
        public Guid PracticeId { get; set; }
        public string PracticeName { get; set; } = string.Empty;
        public int TotalResources { get; set; }
        public int AllocatedResources { get; set; }
        public int AvailableResources { get; set; }
        public int BenchResources { get; set; }
        public decimal UtilizationPercentage { get; set; }
    }
}
