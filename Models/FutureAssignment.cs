using HRMS.Api.Data.Enum;
using HRMS.Api.Enums;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Api.Models
{
    public class FutureAssignment
    {
        [Key]
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public Employee Employee { get; set; } = null!;

        public int? ProjectId { get; set; }

        public Project? Project { get; set; }

        public DateTime? ExpectedStartDate { get; set; }

        public AssignmentProbability Probability { get; set; }
            = AssignmentProbability.Medium;

        [MaxLength(1000)]
        public string? Remarks { get; set; }

        public bool IsConfirmed { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}