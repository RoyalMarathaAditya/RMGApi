using System.ComponentModel.DataAnnotations;

namespace HRMS.Api.Models
{
    public class Practice : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public int? PracticeHeadId { get; set; }
        public Employee? PracticeHead { get; set; }

        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
