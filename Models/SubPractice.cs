using System.ComponentModel.DataAnnotations;

namespace HRMS.Api.Models
{
    public class SubPractice
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public int PracticeId { get; set; }

        public Practice Practice { get; set; } = null!;

        public ICollection<Employee> Employees { get; set; }
            = new List<Employee>();
    }
}