namespace HRMS.Api.DTOs.EmployeeDtos
{
    public class EmployeeBulkUploadResultDto
    {
        public bool Success { get; set; }
        public int TotalRows { get; set; }
        public int SuccessRows { get; set; }
        public int FailedRows { get; set; }
        public int DeletedRows { get; set; }
        public List<EmployeeImportErrorDto> Errors { get; set; } = new();
        public string? ErrorFileUrl { get; set; }
    }

    public class EmployeeImportErrorDto
    {
        public int RowNumber { get; set; }
        public string? EmployeeName { get; set; }
        public string? Email { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
