using System.ComponentModel.DataAnnotations;

namespace HRMS.Api.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [MaxLength(255)]
        public string Description { get; set; } = string.Empty;
    }
}
