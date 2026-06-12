using System.ComponentModel.DataAnnotations;

namespace HRMS.Api.Models
{
    public class Skill
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        public ICollection<ProjectDetailsSkill> ProjectSkills { get; set; } = new List<ProjectDetailsSkill>();
    }
}
