namespace HRMS.Api.DTOs.DashboardDtos
{
    public class PracticeSummaryDto
    {
        public int PracticeId { get; set; }
        public string PracticeName { get; set; } = string.Empty;
        public int EmployeeCount { get; set; }
    }
}
