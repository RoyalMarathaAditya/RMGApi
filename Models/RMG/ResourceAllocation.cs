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

        public Guid? ProjectStatusId { get; set; }
        public ProjectStatusMaster? ProjectStatus { get; set; }

        public Guid? StatusId { get; set; }
        public StatusMaster? Status { get; set; }

        public Guid? ProbableNextAssignmentId { get; set; }
        public ProbableNextAssignmentMaster? ProbableNextAssignment { get; set; }

        public DateTime? ProbableNextAssignmentDate { get; set; }

        public Guid? BillableDateProbabilityId { get; set; }
        public BillableDateProbabilityMaster? BillableDateProbability { get; set; }

        public Guid? CurrentBillingStatusId { get; set; }
        public CurrentBillingStatusMaster? CurrentBillingStatus { get; set; }

        public Guid? BillingBucketId { get; set; }
        public BillingBucketMaster? BillingBucket { get; set; }

        public Guid? AgeingBucketId { get; set; }
        public AgeingBucketMaster? AgeingBucket { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        [MaxLength(50)]
        public string AllocationStatus { get; set; } = "Planned";

        public decimal AllocationPercentage { get; set; }

        [MaxLength(50)]
        public string? AllocationType { get; set; }

        [MaxLength(50)]
        public string? BillableStatus { get; set; }

        [MaxLength(500)]
        public string? ActionItem { get; set; }

        [MaxLength(1000)]
        public string? Remarks { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }

        [MaxLength(10)]
        public string? Engineering { get; set; }

        public int? Duration { get; set; }

        public int? Ageing { get; set; }

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
