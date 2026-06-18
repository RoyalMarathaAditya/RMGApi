using HRMS.Api.Models;

namespace HRMS.Api.DTOs.EmployeeDtos
{
    public class CreateEmployeeHierarchyDto
    {
        public int EmployeeId { get; set; }
        public int ManagerId { get; set; }
        public ManagerType ManagerType { get; set; }
    }
}