namespace HRMS.Api.DTOs.ReportDtos
{
    public class PracticeWiseReportSpResult
    {
        public Guid PracticeId { get; set; }
        public string PracticeName { get; set; } = string.Empty;
        public int TotalHeadcount { get; set; }
        public int BillableCount { get; set; }
        public int UtilizedCount { get; set; }
        public int RangeLessThan1 { get; set; }
        public int Range1to3 { get; set; }
        public int Range3to6 { get; set; }
        public int Range6to9 { get; set; }
        public int Range9to12 { get; set; }
        public int RangeMoreThan12 { get; set; }
    }
}
