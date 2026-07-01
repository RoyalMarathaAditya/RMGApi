namespace HRMS.Api.DTOs.AllocationDtos
{
    public class UpdateEmployeeDetailsDto
    {
        public int EmployeeId { get; set; }
        public decimal ExperienceInNV { get; set; }
        public int? PrimarySkillId { get; set; }
        public List<int> SkillIds { get; set; } = new();
        public string? PrimarySkillName { get; set; }
        public string? SkillNames { get; set; }
        public int? ProjectManagerId { get; set; }
        public bool IsActive { get; set; }
        public string? Remarks { get; set; }
    }
}
