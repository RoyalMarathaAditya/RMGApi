using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.Api.Models
{
    public class ProjectDetailsSkill
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("ProjectDetails")]
        public int ProjectDetailsId { get; set; }
        public ProjectDetails? ProjectDetails { get; set; }

        [ForeignKey("Skill")]
        public int SkillId { get; set; }
        public Skill? Skill { get; set; }

        [MaxLength(50)]
        public string Level { get; set; } = "Intermediate"; // Junior/Mid/Senior
    }
}
