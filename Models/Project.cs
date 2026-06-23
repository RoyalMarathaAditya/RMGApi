using System.ComponentModel.DataAnnotations;

namespace HRMS.Api.Models
{
    public class Project
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string ProjectName { get; set; } = string.Empty;

        public int ClientId { get; set; }
        public Client Client { get; set; } = null!;

        public Guid ProjectTypeId { get; set; }
        public ProjectTypeMaster ProjectType { get; set; } = null!;

        public Guid PricingTypeId { get; set; }
        public PricingTypeMaster PricingType { get; set; } = null!;

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; } = true;

        public Guid PracticeId { get; set; }
        public Practice Practice { get; set; } = null!;

        public int? ProjectManagerId { get; set; }
        public Employee? ProjectManager { get; set; }

        public int? CSMId { get; set; }
        public Employee? CSM { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string? ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; } = default!;

        public ICollection<ProjectAllocation> ProjectAllocations { get; set; } = new List<ProjectAllocation>();
    }
}
