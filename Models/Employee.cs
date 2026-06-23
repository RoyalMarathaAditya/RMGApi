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

        [MaxLength(100)]
        public string? FirstName { get; set; }

        [MaxLength(100)]
        public string? LastName { get; set; }

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
        public decimal PriorExperience { get; set; }
        public decimal? RelevantExperience { get; set; }

        public Guid EmploymentTypeId { get; set; }
        public EmploymentTypeMaster EmploymentType { get; set; } = null!;

        public Guid LocationId { get; set; }
        public Location Location { get; set; } = null!;

        public Guid WorkModelId { get; set; }
        public WorkModelMaster WorkModel { get; set; } = null!;

        public Guid PracticeId { get; set; }
        public Practice Practice { get; set; } = null!;

        public Guid DepartmentTypeId { get; set; }
        public DepartmentTypeMaster DepartmentType { get; set; } = null!;

        public Guid StatusId { get; set; }
        public StatusMaster EmployeeStatus { get; set; } = null!;

        public int? ReportingManagerId { get; set; }
        public Employee? ReportingManager { get; set; }
        public ICollection<Employee> DirectReports { get; set; } = new List<Employee>();

        public int? PracticeHeadId { get; set; }
        public Employee? PracticeHead { get; set; }
        public ICollection<Employee> PracticeHeadEmployees { get; set; } = new List<Employee>();

        public bool? DeloitteFitment { get; set; }
        public bool? Engineering { get; set; }

        [MaxLength(20)]
        public string? MobileNumber { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string? ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; } = default!;

        public ICollection<EmployeeSkill> EmployeeSkills { get; set; } = new List<EmployeeSkill>();
        public ICollection<Project> ManagedProjects { get; set; } = new List<Project>();
        public ICollection<Project> CSMProjects { get; set; } = new List<Project>();
        public ICollection<EmployeeLeave> EmployeeLeaves { get; set; } = new List<EmployeeLeave>();
        public ICollection<PIP> PIPs { get; set; } = new List<PIP>();
        public ICollection<ProjectAllocation> ProjectAllocations { get; set; } = new List<ProjectAllocation>();
    }
}
