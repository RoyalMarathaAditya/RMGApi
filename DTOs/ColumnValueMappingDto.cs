namespace HRMS.Api.DTOs
{
    public class ColumnValueMappingDto
    {
        public Guid Id { get; set; }
        public string TargetProperty { get; set; } = string.Empty;
        public string SourceValue { get; set; } = string.Empty;
        public string TargetValue { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }

    public class CreateColumnValueMappingDto
    {
        public string TargetProperty { get; set; } = string.Empty;
        public string SourceValue { get; set; } = string.Empty;
        public string TargetValue { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }

    public class UpdateColumnValueMappingDto
    {
        public string TargetProperty { get; set; } = string.Empty;
        public string SourceValue { get; set; } = string.Empty;
        public string TargetValue { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
