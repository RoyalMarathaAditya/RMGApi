namespace HRMS.Api.DTOs.EmployeeDtos
{
    public class EmployeeHierarchyDto
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;

        public int ManagementLeaderId { get; set; }
        public string ManagementLeaderName { get; set; } = string.Empty;

        public string ManagerType { get; set; } = string.Empty;
    }
}

