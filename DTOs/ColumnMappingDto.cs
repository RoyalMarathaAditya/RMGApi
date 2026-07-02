namespace HRMS.Api.DTOs
{
    public class ColumnMappingDto
    {
        public Guid Id { get; set; }
        public string SourceColumn { get; set; } = string.Empty;
        public string TargetProperty { get; set; } = string.Empty;
        public string TargetDisplayName { get; set; } = string.Empty;
        public string DataType { get; set; } = "string";
        public bool IsRequired { get; set; }
        public bool IsActive { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }

    public class CreateColumnMappingDto
    {
        public string SourceColumn { get; set; } = string.Empty;
        public string TargetProperty { get; set; } = string.Empty;
        public string TargetDisplayName { get; set; } = string.Empty;
        public string DataType { get; set; } = "string";
        public bool IsRequired { get; set; }
        public bool IsActive { get; set; } = true;
        public int DisplayOrder { get; set; }
    }

    public class UpdateColumnMappingDto
    {
        public string SourceColumn { get; set; } = string.Empty;
        public string TargetProperty { get; set; } = string.Empty;
        public string TargetDisplayName { get; set; } = string.Empty;
        public string DataType { get; set; } = "string";
        public bool IsRequired { get; set; }
        public bool IsActive { get; set; }
        public int DisplayOrder { get; set; }
    }
}
