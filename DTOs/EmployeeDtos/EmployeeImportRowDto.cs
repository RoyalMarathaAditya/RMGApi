namespace HRMS.Api.DTOs.EmployeeDtos
{
    public class EmployeeImportRowDto
    {
        public int RowNumber { get; set; }
        public string? EmployeeCode { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; }
        public string EmployeeType { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public string? PracticeHead { get; set; }
        public string? ReportingManager { get; set; }
        public string Practice { get; set; } = string.Empty;
        public string? Client { get; set; }
        public DateTime? DOJ { get; set; }
        public string? NVLocation { get; set; }
        public string? WorkModel { get; set; }
        public string? Onboarding { get; set; }
        public string? PhoneNumber { get; set; }
        public string Email { get; set; } = string.Empty;
        public decimal? Experience { get; set; }
    }
}
