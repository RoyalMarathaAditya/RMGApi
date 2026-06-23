namespace HRMS.Api.DTOs.EmployeeDtos
{
    public class UpdateEmployeeDto
    {
        public string EmployeeCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime DOJ { get; set; }
        public DateTime? LWD { get; set; }
        public decimal PriorExperience { get; set; }
        public decimal? RelevantExperience { get; set; }
        public Guid EmploymentTypeId { get; set; }
        public Guid LocationId { get; set; }
        public Guid WorkModelId { get; set; }
        public Guid PracticeId { get; set; }
        public Guid DepartmentTypeId { get; set; }
        public Guid StatusId { get; set; }
        public int? ReportingManagerId { get; set; }
        public int? PracticeHeadId { get; set; }
        public bool? DeloitteFitment { get; set; }
        public bool? Engineering { get; set; }
        public string? MobileNumber { get; set; }
        public Guid? DesignationId { get; set; }
        public List<Guid> SkillIds { get; set; } = new();
    }
}
