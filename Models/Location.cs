using System.ComponentModel.DataAnnotations;

namespace HRMS.Api.Models
{
    public class Location
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Address { get; set; } = string.Empty;

        public ICollection<Project> Projects { get; set; } = new List<Project>();

        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
