using System.Text.Json;

namespace HRMS.Api.DTOs.EmployeeDtos
{
    public class EmployeeImportRowDto
    {
        public int RowNumber { get; set; }
        public string? EmployeeCode { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string EmployeeType { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public string Practice { get; set; } = string.Empty;
        public string? SubPractice { get; set; }
        public string? NVLocation { get; set; }
        public string? ReportingManager { get; set; }
        public string? PracticeHead { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? ActiveStatus { get; set; }
        public DateTime? DOJ { get; set; }
        public DateTime? LWD { get; set; }
        public Dictionary<string, string> AdditionalFields { get; set; } = new();
    }
}
