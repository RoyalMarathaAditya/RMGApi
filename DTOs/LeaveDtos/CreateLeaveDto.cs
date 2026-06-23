using System.ComponentModel.DataAnnotations;

namespace HRMS.Api.DTOs.LeaveDtos
{
    public class CreateLeaveDto
    {
        [Required] public int EmployeeId { get; set; }
        [Required] public Guid LeaveTypeId { get; set; }
        [Required] public DateTime FromDate { get; set; }
        [Required] public DateTime ToDate { get; set; }
        [Required] public int NumberOfDays { get; set; }
        public string? Remarks { get; set; }
    }
}
