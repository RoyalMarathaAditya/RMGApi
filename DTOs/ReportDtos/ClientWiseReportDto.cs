namespace HRMS.Api.DTOs.ReportDtos
{
    public class ClientWiseReportDto
    {
        public int ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public int TotalProjects { get; set; }
        public int ActiveProjects { get; set; }
        public int CompletedProjects { get; set; }
        public int TotalEmployees { get; set; }
        public int BillableCount { get; set; }
        public int NonBillableCount { get; set; }
        public decimal AvgAllocation { get; set; }
        public List<ClientProjectDto> Projects { get; set; } = new();
    }

    public class ClientProjectDto
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int EmployeeCount { get; set; }
        public decimal AvgAllocation { get; set; }
    }
}
