//using System.ComponentModel.DataAnnotations;

//namespace HRMS.Api.Models
//{
//    public class Skill
//    {
//        [Key]
//        public int Id { get; set; }

//        [Required]
//        [MaxLength(100)]
//        public string Name { get; set; } = string.Empty;

//        [MaxLength(500)]
//        public string Description { get; set; } = string.Empty;

//        public ICollection<ProjectSkill> ProjectSkills { get; set; } = new List<ProjectSkill>();

//    }
//}

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
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        // Employee Skills
        public ICollection<EmployeeSkill> EmployeeSkills { get; set; } = new List<EmployeeSkill>();

        // Optional - Project Skills
        public ICollection<ProjectSkill> ProjectSkills { get; set; } = new List<ProjectSkill>();



    }
}
