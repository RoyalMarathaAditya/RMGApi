namespace HRMS.Api.DTOs.ReportDtos
{
    public class ProjectWiseReportDto
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string? ProjectCode { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string? ProjectManager { get; set; }
        public DateTime ProjectStartDate { get; set; }
        public DateTime? ProjectEndDate { get; set; }
        public int TotalEmployees { get; set; }
        public int BillableCount { get; set; }
        public decimal AvgAllocation { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<ProjectEmployeeDto> Employees { get; set; } = new();
    }

    public class ProjectEmployeeDto
    {
        public string EmployeeCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public decimal AllocationPercentage { get; set; }
        public string? BillableStatus { get; set; }
    }
}
