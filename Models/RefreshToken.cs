using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.Api.Models
{
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Token { get; set; } = string.Empty;

        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User? User { get; set; }
    }
}
