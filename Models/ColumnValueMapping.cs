using System.ComponentModel.DataAnnotations;

namespace HRMS.Api.Models
{
    public class ColumnValueMapping
    {
        [Key]
        public Guid Id { get; set; }

        [MaxLength(200)]
        public string TargetProperty { get; set; } = string.Empty;

        [MaxLength(200)]
        public string SourceValue { get; set; } = string.Empty;

        [MaxLength(200)]
        public string TargetValue { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }
    }
}
