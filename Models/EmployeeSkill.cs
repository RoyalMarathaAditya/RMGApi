using System.ComponentModel.DataAnnotations;

namespace HRMS.Api.Models
{
    public class EmployeeSkill
    {
        [Key]
        public int Id { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;

        public Guid SkillId { get; set; }
        public Skill Skill { get; set; } = null!;

        public decimal? RelevantExperience { get; set; }
    }
}
