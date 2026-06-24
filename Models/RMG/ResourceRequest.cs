using System.ComponentModel.DataAnnotations;

namespace HRMS.Api.Models.RMG
{
    public class ResourceRequest
    {
        [Key]
        public int Id { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        public int RequestedById { get; set; }
        public Employee RequestedBy { get; set; } = null!;

        public Guid? PracticeId { get; set; }
        public Practice? Practice { get; set; }

        [MaxLength(500)]
        public string? RequiredSkillIds { get; set; }

        public int RequiredCount { get; set; } = 1;

        public DateTime RequiredByDate { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "Draft";

        [MaxLength(50)]
        public string? Priority { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        [MaxLength(100)]
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        [MaxLength(100)]
        public string? ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; } = null!;
    }
}
