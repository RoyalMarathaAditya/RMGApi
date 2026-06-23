using System.ComponentModel.DataAnnotations;

namespace HRMS.Api.Models
{
    public class ProjectAllocation
    {
        [Key]
        public int Id { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;

        public int ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        public Guid EmployeeProjectStatusId { get; set; }
        public EmployeeProjectStatusMaster EmployeeProjectStatus { get; set; } = null!;

        public Guid AllocationStatusId { get; set; }
        public AllocationStatusMaster AllocationStatus { get; set; } = null!;

        public DateTime AllocationStartDate { get; set; }
        public DateTime? AllocationEndDate { get; set; }

        [Range(0, 100)]
        public decimal AllocationPercentage { get; set; }

        [Range(0, 100)]
        public decimal? BillablePercentage { get; set; }

        public bool IsBillable { get; set; }
        public bool IsUtilized { get; set; }

        public DateTime? ProbableBillableDate { get; set; }
        public DateTime? NextAssignmentDate { get; set; }

        [MaxLength(1000)]
        public string? Remarks { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string? ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; } = default!;
    }
}
