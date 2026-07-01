using System.ComponentModel.DataAnnotations;

namespace HRMS.Api.Models
{
    public class CSMRevenueType
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string? ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; } = default!;

        public ICollection<Project> Projects { get; set; } = new List<Project>();
    }
}
