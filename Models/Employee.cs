using System.ComponentModel.DataAnnotations;

namespace HRMS.Api.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(120)]
        public string FirstName { get; set; } = null!;

        [MaxLength(120)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Email { get; set; } = null!;

        [MaxLength(50)]
        public string Department { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Designation { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Status { get; set; } = "Active";

        public DateTime DateOfJoining { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
