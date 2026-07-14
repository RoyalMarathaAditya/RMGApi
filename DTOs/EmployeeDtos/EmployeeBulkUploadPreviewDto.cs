namespace HRMS.Api.DTOs.EmployeeDtos
{
    public class EmployeeBulkUploadPreviewDto
    {
        public bool Success { get; set; } = true;
        public string? ErrorMessage { get; set; }
        public int TotalRows { get; set; }
        public int NewEmployees { get; set; }
        public int UpdatedEmployees { get; set; }
        public int DeletedEmployees { get; set; }
        public List<EmployeeChangeDto> Changes { get; set; } = new();
        public List<SimpleEmployeeInfo> NewEmployeeList { get; set; } = new();
        public List<SimpleEmployeeInfo> DeletedEmployeeList { get; set; } = new();
    }

    public class EmployeeChangeDto
    {
        public string Email { get; set; } = string.Empty;
        public string EmployeeCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public List<FieldChangeDto> FieldChanges { get; set; } = new();
    }

    public class FieldChangeDto
    {
        public string FieldName { get; set; } = string.Empty;
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
    }

    public class SimpleEmployeeInfo
    {
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
    }
}
