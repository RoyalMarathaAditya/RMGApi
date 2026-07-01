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

        [MaxLength(50)]
        public string? ProjectCode { get; set; }

        public int ClientId { get; set; }
        public Client Client { get; set; } = null!;

        [MaxLength(200)]
        public string? ProjectManager { get; set; }

        [MaxLength(200)]
        public string? DeliveryHead { get; set; }

        public Guid? CSMRevenueTypeId { get; set; }
        public CSMRevenueType? CSMRevenueType { get; set; }

        public bool IsActive { get; set; } = true;

        [MaxLength(1000)]
        public string? Description { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string? ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; } = default!;
    }
}
