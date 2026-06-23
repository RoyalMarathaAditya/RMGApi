using System.ComponentModel.DataAnnotations;

namespace HRMS.Api.Models
{
    public abstract class BaseMasterEntity : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}
