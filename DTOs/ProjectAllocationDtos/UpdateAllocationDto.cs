namespace HRMS.Api.DTOs.ProjectAllocationDtos
{
    public class UpdateAllocationDto
    {
        public int EmployeeId { get; set; }
        public int ProjectId { get; set; }
        public Guid EmployeeProjectStatusId { get; set; }
        public Guid AllocationStatusId { get; set; }
        public DateTime AllocationStartDate { get; set; }
        public DateTime? AllocationEndDate { get; set; }
        public decimal AllocationPercentage { get; set; }
        public decimal? BillablePercentage { get; set; }
        public bool IsBillable { get; set; }
        public bool IsUtilized { get; set; }
        public DateTime? ProbableBillableDate { get; set; }
        public DateTime? NextAssignmentDate { get; set; }
        public string? Remarks { get; set; }
    }
}
