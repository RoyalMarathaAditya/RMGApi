using System.ComponentModel.DataAnnotations;

namespace HRMS.Api.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string EmployeeCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        public decimal? ExperienceYears { get; set; }

        public Guid? DesignationId { get; set; }
        public DesignationMaster? Designation { get; set; }

        public int? ClientId { get; set; }
        public Client? Client { get; set; }

        public Guid? OnboardingTypeId { get; set; }
        public OnboardingTypeMaster? OnboardingType { get; set; }

        public DateTime DOJ { get; set; }
        public DateTime? LWD { get; set; }
        public decimal? PriorExperience { get; set; }
        public decimal? RelevantExperience { get; set; }

        public Guid EmploymentTypeId { get; set; }
        public EmploymentTypeMaster EmploymentType { get; set; } = null!;

        public Guid LocationId { get; set; }
        public Location Location { get; set; } = null!;

        public Guid? WorkModelId { get; set; }
        public WorkModelMaster? WorkModel { get; set; }

        public Guid PracticeId { get; set; }
        public Practice Practice { get; set; } = null!;

        public Guid? SubPracticeId { get; set; }
        public SubPracticeMaster? SubPractice { get; set; }

        public Guid? DepartmentTypeId { get; set; }
        public DepartmentTypeMaster? DepartmentType { get; set; }

        public Guid StatusId { get; set; }
        public StatusMaster EmployeeStatus { get; set; } = null!;

        [MaxLength(200)]
        public string? ReportingManagerName { get; set; }

        public int? ReportingManagerId { get; set; }
        public Employee? ReportingManager { get; set; }
        public ICollection<Employee> DirectReports { get; set; } = new List<Employee>();

        [MaxLength(200)]
        public string? PracticeHeadName { get; set; }

        public int? PracticeHeadId { get; set; }
        public Employee? PracticeHead { get; set; }
        public ICollection<Employee> PracticeHeadEmployees { get; set; } = new List<Employee>();

        public bool? DeloitteFitment { get; set; }
        public bool? Engineering { get; set; }

        [MaxLength(20)]
        public string? MobileNumber { get; set; }

        [MaxLength(1000)]
        public string? Remarks { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string? ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; } = default!;

        public ICollection<EmployeeSkill> EmployeeSkills { get; set; } = new List<EmployeeSkill>();
        public ICollection<EmployeeLeave> EmployeeLeaves { get; set; } = new List<EmployeeLeave>();
        public ICollection<PIP> PIPs { get; set; } = new List<PIP>();
    }
}
