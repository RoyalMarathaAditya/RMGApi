namespace HRMS.Api.DTOs.AllocationDtos
{
    public class CalendarViewDto
    {
        public int AllocationId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal AllocationPercentage { get; set; }
        public string AllocationStatus { get; set; } = string.Empty;
        public string ColorCode { get; set; } = string.Empty;
    }

    public class TimelineViewDto
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public List<TimelineEmployeeDto> Employees { get; set; } = new();
    }

    public class TimelineEmployeeDto
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal AllocationPercentage { get; set; }
        public string AllocationStatus { get; set; } = string.Empty;
    }
}
