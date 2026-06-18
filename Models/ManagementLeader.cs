using System.ComponentModel.DataAnnotations;

namespace HRMS.Api.Models
{
    public class ManagementLeader
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string LeaderCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string FullName { get; set; } = string.Empty;

        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty; // L1, L2, PM, RM

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}