using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.Api.Models
{
    public class ProjectDetails
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime? EndDate { get; set; }

        [ForeignKey("Client")]
        public int ClientId { get; set; }
        public Client? Client { get; set; }

        [ForeignKey("Location")]
        public int LocationId { get; set; }
        public Location? Location { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<ProjectDetailsSkill> ProjectSkills { get; set; } = new List<ProjectDetailsSkill>();
    }
}
