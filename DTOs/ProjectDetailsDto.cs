using System.ComponentModel.DataAnnotations;

namespace HRMS.Api.DTOs
{
    public class ProjectDto
    {
        public int Id { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string? ProjectCode { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string? ProjectManager { get; set; }
        public string? DeliveryHead { get; set; }
        public string? CSM { get; set; }
        public Guid? CSMRevenueTypeId { get; set; }
        public string? CSMRevenueTypeName { get; set; }
        public DateTime ProjectStartDate { get; set; }
        public DateTime ProjectEndDate { get; set; }
        public bool IsActive { get; set; }
        public string? Description { get; set; }
    }

    public class CreateProjectDto
    {
        [Required]
        [MaxLength(200)]
        public string ProjectName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? ProjectCode { get; set; }

        public int ClientId { get; set; }

        [MaxLength(200)]
        public string? ProjectManager { get; set; }

        [MaxLength(200)]
        public string? DeliveryHead { get; set; }

        [MaxLength(200)]
        public string? CSM { get; set; }

        public Guid? CSMRevenueTypeId { get; set; }

        public DateTime ProjectStartDate { get; set; }

        public DateTime ProjectEndDate { get; set; }

        public bool IsActive { get; set; } = true;

        [MaxLength(1000)]
        public string? Description { get; set; }
    }

    public class UpdateProjectDto
    {
        [Required]
        [MaxLength(200)]
        public string ProjectName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? ProjectCode { get; set; }

        public int ClientId { get; set; }

        [MaxLength(200)]
        public string? ProjectManager { get; set; }

        [MaxLength(200)]
        public string? DeliveryHead { get; set; }

        [MaxLength(200)]
        public string? CSM { get; set; }

        public Guid? CSMRevenueTypeId { get; set; }

        public DateTime ProjectStartDate { get; set; }

        public DateTime ProjectEndDate { get; set; }

        public bool IsActive { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }
    }

    public class CSMRevenueTypeDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
