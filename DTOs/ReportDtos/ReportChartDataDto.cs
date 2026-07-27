namespace HRMS.Api.DTOs.ReportDtos
{
    public class ReportChartDataDto
    {
        public List<DonutDataDto> BillableDistribution { get; set; } = new();
        public List<BarDataDto> UtilizationByEntity { get; set; } = new();
        public List<PieDataDto> ProjectsByStatus { get; set; } = new();
        public List<LineDataDto> MonthlyTrend { get; set; } = new();
    }

    public class DonutDataDto
    {
        public string Name { get; set; } = string.Empty;
        public int Value { get; set; }
    }

    public class BarDataDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal Utilization { get; set; }
    }

    public class PieDataDto
    {
        public string Name { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class LineDataDto
    {
        public string Month { get; set; } = string.Empty;
        public int Started { get; set; }
        public int Ended { get; set; }
    }
}
