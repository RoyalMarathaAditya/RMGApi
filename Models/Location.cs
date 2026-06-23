using System.ComponentModel.DataAnnotations;

namespace HRMS.Api.Models
{
    public class Location : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Address { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public ICollection<Project> Projects { get; set; } = new List<Project>();
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
