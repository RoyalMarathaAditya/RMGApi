namespace HRMS.Api.DTOs.EmployeeDtos
{
    public class EmployeeDto
    {
        public int Id { get; set; }

        public string EmployeeCode { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public DateTime DateOfJoining { get; set; }

        public decimal PriorExperience { get; set; }

        public decimal CompanyExperience { get; set; }

        public decimal TotalExperience { get; set; }

        public string Status { get; set; } = string.Empty;

        public string Designation { get; set; } = string.Empty;

        public string Practice { get; set; } = string.Empty;

        public string? SubPractice { get; set; }

        public string Location { get; set; } = string.Empty;

        public string? ManagerName { get; set; }

       // public List<EmployeeHierarchyDto> Hierarchies { get; set; } = new();
    }
}