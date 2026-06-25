using System.ComponentModel.DataAnnotations;

namespace HRMS.Api.Models.RMG
{
    public class ResourceAllocation
    {
        [Key]
        public int Id { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;

        public int ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        public int? ClientId { get; set; }
        public Client? Client { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        [MaxLength(50)]
        public string AllocationStatus { get; set; } = "Planned";

        public decimal AllocationPercentage { get; set; }

        [MaxLength(50)]
        public string? AllocationType { get; set; }

        [MaxLength(50)]
        public string? BillableStatus { get; set; }

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
