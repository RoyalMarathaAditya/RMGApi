namespace HRMS.Api.DTOs.DashboardDtos
{
    public class AllocationSummaryDto
    {
        public int CurrentAllocations { get; set; }
        public int UpcomingReleases { get; set; }
        public int LongLeaveEmployees { get; set; }
        public int PipEmployees { get; set; }
    }
}
