namespace HRMS.Api.DTOs.AllocationDtos
{
    public class EmployeeResourceDetailsDto
    {
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Role { get; set; }
        public string? Practice { get; set; }
        public string? SubPractice { get; set; }
        public string? PrimarySkill { get; set; }
        public string? Skill { get; set; }
        public bool? Active { get; set; }
        public string? Location { get; set; }
        public string? L1Manager { get; set; }
        public string? PracticeHead { get; set; }
        public DateTime? DOJ { get; set; }

        public decimal PriorExperience { get; set; }
        public decimal NVExperience { get; set; }
        public decimal TotalExperience { get; set; }
        public string ExperienceRange { get; set; } = string.Empty;

        public string? FteConsultant { get; set; }
        public string? Utilised { get; set; }
        public string? Billable { get; set; }
        public string? Status { get; set; }

        public int? ProjectManagerId { get; set; }
        public string? ProjectManagerName { get; set; }
        public string? Remarks { get; set; }

        public List<ProjectAllocationDetailDto> ProjectAllocations { get; set; } = new();
    }

    public class ProjectAllocationDetailDto
    {
        public string? ProjectCode { get; set; }
        public string? Client { get; set; }
        public string? Project { get; set; }
        public string? ProjectStatus { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? AllocationPercentage { get; set; }
        public decimal? BillablePercentage { get; set; }
        public string? Engineering { get; set; }
        public string? DurationInProject { get; set; }
        public string? Ageing { get; set; }
        public string? Remarks { get; set; }
    }


}
