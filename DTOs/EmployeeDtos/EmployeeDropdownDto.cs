namespace HRMS.Api.DTOs.EmployeeDtos
{
    public class EmployeeDropdownDto
    {
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? MobileNumber { get; set; }
    }
}
