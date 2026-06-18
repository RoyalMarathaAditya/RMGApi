using System.ComponentModel.DataAnnotations;

namespace HRMS.Api.DTOs.DesignationDtos
{
    public class UpdateDesignationDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; }
    }
}