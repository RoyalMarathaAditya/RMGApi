//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace HRMS.Api.Models
//{
//    public class ProjectSkill
//    {
//        [Key]
//        public int Id { get; set; }

//        [ForeignKey("Project")]
//        public int ProjectId { get; set; }
//        public Project? Project { get; set; }

//        [ForeignKey("Skill")]
//        public int SkillId { get; set; }
//        public Skill? Skill { get; set; }

//        [MaxLength(50)]
//        public string Level { get; set; } = "Intermediate"; // Junior/Mid/Senior
//    }
//}
using System.ComponentModel.DataAnnotations;
using HRMS.Api.Enums;

namespace HRMS.Api.Models
{
    public class ProjectSkill
    {
        [Key]
        public int Id { get; set; }

        public int ProjectId { get; set; }

        public Project Project { get; set; } = null!;

        public int SkillId { get; set; }

        public Skill Skill { get; set; } = null!;

        public SkillLevel Level { get; set; }
            = SkillLevel.Intermediate;
    }
}