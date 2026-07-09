using System.ComponentModel.DataAnnotations;

namespace HRMS.Api.Models
{
    public class ColumnMapping
    {
        [Key]
        public Guid Id { get; set; }

        [MaxLength(200)]
        public string SourceColumn { get; set; } = string.Empty;

        [MaxLength(200)]
        public string TargetProperty { get; set; } = string.Empty;

        [MaxLength(200)]
        public string TargetDisplayName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string DataType { get; set; } = "string";

        [MaxLength(100)]
        public string EntityType { get; set; } = "employee-import";

        public bool IsRequired { get; set; }

        public bool IsActive { get; set; } = true;

        public int DisplayOrder { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }
    }
}
