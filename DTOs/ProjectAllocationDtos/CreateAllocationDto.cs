using System.ComponentModel.DataAnnotations;

namespace HRMS.Api.DTOs.ProjectAllocationDtos
{
    public class CreateAllocationDto
    {
        [Required]
        public int EmployeeId { get; set; }

        [Required]
        public int ProjectId { get; set; }

        [Required]
        public Guid EmployeeProjectStatusId { get; set; }

        [Required]
        public Guid AllocationStatusId { get; set; }

        [Required]
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

        public string? Remarks { get; set; }
    }
}
