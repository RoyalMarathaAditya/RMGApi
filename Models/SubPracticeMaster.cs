using System.ComponentModel.DataAnnotations;

namespace HRMS.Api.Models
{
    public class SubPracticeMaster : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public Guid PracticeId { get; set; }
        public Practice Practice { get; set; } = null!;

        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
