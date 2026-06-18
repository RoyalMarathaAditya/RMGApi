using System.ComponentModel.DataAnnotations;

namespace HRMS.Api.DTOs.DesignationDtos
{
    public class CreateDesignationDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
    }
}