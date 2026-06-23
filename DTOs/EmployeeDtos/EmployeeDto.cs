using HRMS.Api.DTOs.MasterDtos;

namespace HRMS.Api.DTOs.EmployeeDtos
{
    public class EmployeeDto
    {
        public int Id { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime DOJ { get; set; }
        public DateTime? LWD { get; set; }
        public decimal PriorExperience { get; set; }
        public decimal? RelevantExperience { get; set; }
        public string? MobileNumber { get; set; }
        public bool? DeloitteFitment { get; set; }
        public bool? Engineering { get; set; }
        public Guid EmploymentTypeId { get; set; }
        public string EmploymentType { get; set; } = string.Empty;
        public Guid LocationId { get; set; }
        public string Location { get; set; } = string.Empty;
        public Guid WorkModelId { get; set; }
        public string WorkModel { get; set; } = string.Empty;
        public Guid PracticeId { get; set; }
        public string Practice { get; set; } = string.Empty;
        public Guid DepartmentTypeId { get; set; }
        public string DepartmentType { get; set; } = string.Empty;
        public Guid StatusId { get; set; }
        public string EmployeeStatus { get; set; } = string.Empty;
        public int? ReportingManagerId { get; set; }
        public string? ReportingManagerName { get; set; }
        public int? PracticeHeadId { get; set; }
        public string? PracticeHeadName { get; set; }
        public Guid? DesignationId { get; set; }
        public string? Designation { get; set; }
        public List<MasterDto> Skills { get; set; } = new();
    }
}
