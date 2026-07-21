namespace HRMS.Api.DTOs.ReportDtos
{
    public class PracticeWiseReportDto
    {
        public Guid PracticeId { get; set; }
        public string PracticeName { get; set; } = string.Empty;
        public int TotalHeadcount { get; set; }
        public int BillableCount { get; set; }
        public int NonBillableCount => TotalHeadcount - BillableCount;
        public int UtilizedCount { get; set; }
        public int NonUtilizedCount => TotalHeadcount - UtilizedCount;
        public decimal BillabilityPercentage => TotalHeadcount > 0 ? Math.Round((decimal)BillableCount / TotalHeadcount * 100, 2) : 0;
        public decimal UtilizationPercentage => TotalHeadcount > 0 ? Math.Round((decimal)UtilizedCount / TotalHeadcount * 100, 2) : 0;
        public List<ExperienceRangeDto> ExperienceRanges { get; set; } = new();
    }

    public class ExperienceRangeDto
    {
        public string Range { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
