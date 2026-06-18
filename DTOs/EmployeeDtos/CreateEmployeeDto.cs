using System.ComponentModel.DataAnnotations;

namespace HRMS.Api.DTOs.EmployeeDtos
{
    public class CreateEmployeeDto
    {
        [Required]
        public string EmployeeCode { get; set; } = string.Empty;

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        public DateTime DateOfJoining { get; set; }

        public decimal PriorExperience { get; set; }

        public int DesignationId { get; set; }
        public int PracticeId { get; set; }
        public int? SubPracticeId { get; set; }
        public int LocationId { get; set; }
        public int? ManagerId { get; set; }

        public string Status { get; set; } = "Active";
    }
}