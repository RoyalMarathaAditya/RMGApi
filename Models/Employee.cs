////using System.ComponentModel.DataAnnotations;

////namespace HRMS.Api.Models
////{
////    public class Employee
////    {
////        [Key]
////        public int Id { get; set; }

////        [Required]
////        [MaxLength(120)]
////        public string FirstName { get; set; } = null!;

////        [MaxLength(120)]
////        public string LastName { get; set; } = string.Empty;

////        [Required]
////        [MaxLength(100)]
////        public string Email { get; set; } = null!;

////        [MaxLength(50)]
////        public string Department { get; set; } = string.Empty;

////        [MaxLength(50)]
////        public string Designation { get; set; } = string.Empty;

////        [MaxLength(20)]
////        public string Status { get; set; } = "Active";

////        public DateTime DateOfJoining { get; set; } = DateTime.UtcNow;
////        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
////        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
////    }
////}


//using System.ComponentModel.DataAnnotations;
//using System.Reflection;

//namespace HRMS.Api.Models
//{
//    public class Employee
//    {
//        [Key]
//        public int Id { get; set; }

//        [Required]
//        [MaxLength(50)]
//        public string EmployeeCode { get; set; } = string.Empty;

//        [Required]
//        [MaxLength(200)]
//        public string FullName { get; set; } = string.Empty;

//        [Required]
//        [MaxLength(150)]
//        public string Email { get; set; } = string.Empty;

//        public DateTime DateOfJoining { get; set; }

//        public decimal PriorExperience { get; set; }

//        public decimal CompanyExperience { get; set; }

//        public decimal TotalExperience { get; set; }

//        [MaxLength(20)]
//        public string Status { get; set; } = "Active";

//        // FK
//        public int DesignationId { get; set; }

//        public int PracticeId { get; set; }

//        public int? SubPracticeId { get; set; }

//        public int LocationId { get; set; }

//        public int? ManagerId { get; set; }

//        // Navigation Properties
//        public Designation Designation { get; set; } = null!;

//        public Practice Practice { get; set; } = null!;

//        public SubPractice? SubPractice { get; set; }

//        public Location Location { get; set; } = null!;

//        public Employee? Manager { get; set; }

//        public ICollection<Employee> DirectReports { get; set; }
//            = new List<Employee>();

//        public ICollection<EmployeeSkill> EmployeeSkills { get; set; }
//            = new List<EmployeeSkill>();

//        public ICollection<ResourceAllocation> ResourceAllocations { get; set; }
//            = new List<ResourceAllocation>();

//        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

//        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
//    }
//}

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

        public DateTime DateOfJoining { get; set; }

        public decimal PriorExperience { get; set; }

        public decimal CompanyExperience { get; set; }

        public decimal TotalExperience { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = "Active";

        // Foreign Keys
        public int DesignationId { get; set; }

        public int PracticeId { get; set; }

        public int? SubPracticeId { get; set; }

        public int LocationId { get; set; }

        public int? ManagerId { get; set; }

        // Navigation Properties
        public Designation Designation { get; set; } = null!;

        public Practice Practice { get; set; } = null!;

        public SubPractice? SubPractice { get; set; }

        public Location Location { get; set; } = null!;

        public Employee? Manager { get; set; }

        public ICollection<Employee> DirectReports { get; set; }
            = new List<Employee>();

        public ICollection<EmployeeSkill> EmployeeSkills { get; set; }
            = new List<EmployeeSkill>();

        public ICollection<ResourceAllocation> ResourceAllocations { get; set; }
            = new List<ResourceAllocation>();

        public ICollection<Project> ManagedProjects { get; set; }
    = new List<Project>();

        public ICollection<Project> DeliveryProjects { get; set; }
            = new List<Project>();
        public ICollection<FutureAssignment> FutureAssignments { get; set; }
    = new List<FutureAssignment>();

    //    public ICollection<EmployeeHierarchy> EmployeeHierarchies { get; set; }
    //= new List<EmployeeHierarchy>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}