namespace HRMS.Api.DTOs.DashboardDtos
{
    public class ResourceSummaryDto
    {
        public int TotalEmployees { get; set; }
        public int ActiveEmployees { get; set; }
        public int BenchEmployees { get; set; }
        public int BillableEmployees { get; set; }
        public decimal UtilizedPercentage { get; set; }
    }
}
