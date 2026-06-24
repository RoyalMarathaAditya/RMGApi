using System.ComponentModel.DataAnnotations;

namespace HRMS.Api.Models.RMG
{
    public class ResourceAllocationHistory
    {
        [Key]
        public int Id { get; set; }

        public int ResourceAllocationId { get; set; }

        public int EmployeeId { get; set; }

        public int? OldProjectId { get; set; }
        public int? NewProjectId { get; set; }

        public decimal? OldAllocationPercentage { get; set; }
        public decimal? NewAllocationPercentage { get; set; }

        [MaxLength(50)]
        public string? OldAllocationStatus { get; set; }
        [MaxLength(50)]
        public string? NewAllocationStatus { get; set; }

        [MaxLength(50)]
        public string ChangeType { get; set; } = string.Empty;

        [MaxLength(100)]
        public string ModifiedBy { get; set; } = string.Empty;

        public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;

        [MaxLength(1000)]
        public string? Remarks { get; set; }
    }
}
