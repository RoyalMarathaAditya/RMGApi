//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace HRMS.Api.Models
//{
//    public class Project
//    {
//        [Key]
//        public int Id { get; set; }

//        [Required]
//        [MaxLength(200)]
//        public string Name { get; set; } = string.Empty;

//        [MaxLength(1000)]
//        public string Description { get; set; } = string.Empty;

//        public DateTime StartDate { get; set; } = DateTime.UtcNow;
//        public DateTime? EndDate { get; set; }

//        [ForeignKey("Client")]
//        public int ClientId { get; set; }
//        public Client? Client { get; set; }

//        [ForeignKey("Location")]
//        public int LocationId { get; set; }
//        public Location? Location { get; set; }

//        public bool IsActive { get; set; } = true;

//        public ICollection<ProjectSkill> ProjectSkills { get; set; } = new List<ProjectSkill>();
//    }
//}

using System.ComponentModel.DataAnnotations;

namespace HRMS.Api.Models
{
    public class Project
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string ProjectCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        public int ClientId { get; set; }

        public Client Client { get; set; } = null!;

        public int LocationId { get; set; }

        public Location Location { get; set; } = null!;

        // Project Manager
        public int? ProjectManagerId { get; set; }

        public Employee? ProjectManager { get; set; }

        // Delivery Head
        public int? DeliveryHeadId { get; set; }

        public Employee? DeliveryHead { get; set; }

        [MaxLength(100)]
        public string? RevenueType { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "Active";

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<ProjectSkill> ProjectSkills { get; set; }
            = new List<ProjectSkill>();

        public ICollection<ResourceAllocation> ResourceAllocations { get; set; }
            = new List<ResourceAllocation>();
        public ICollection<FutureAssignment> FutureAssignments { get; set; }
            = new List<FutureAssignment>();
    }
}