using HRMS.Api.Data.Enum;
using HRMS.Api.Enums;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Api.Models
{
    public class ResourceAllocation
    {
        [Key]
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public Employee Employee { get; set; } = null!;

        public int ProjectId { get; set; }

        public Project Project { get; set; } = null!;

        [Range(0, 100)]
        public decimal AllocationPercentage { get; set; }

        [Range(0, 100)]
        public decimal? BillablePercentage { get; set; }

        public bool IsBillable { get; set; }

        public bool IsUtilized { get; set; }

        public AllocationStatus AllocationStatus { get; set; }
            = AllocationStatus.Allocated;

        public BillingStatus BillingStatus { get; set; }
            = BillingStatus.Billable;

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [MaxLength(1000)]
        public string? Remarks { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}