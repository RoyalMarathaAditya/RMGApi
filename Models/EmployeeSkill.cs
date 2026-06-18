using System.ComponentModel.DataAnnotations;
using HRMS.Api.Enums;

namespace HRMS.Api.Models
{
    public class EmployeeSkill
    {
        [Key]
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public Employee Employee { get; set; } = null!;

        public int SkillId { get; set; }

        public Skill Skill { get; set; } = null!;

        public SkillLevel SkillLevel { get; set; } = SkillLevel.Intermediate;

        public decimal ExperienceInYears { get; set; }

        public bool IsPrimarySkill { get; set; } = false;
    }
}