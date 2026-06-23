using System.ComponentModel.DataAnnotations;

namespace HRMS.Api.DTOs.PIPDtos
{
    public class CreatePipDto
    {
        [Required] public int EmployeeId { get; set; }
        [Required] public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        [Required] public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = "Active";
        public string? Remarks { get; set; }
    }
}
