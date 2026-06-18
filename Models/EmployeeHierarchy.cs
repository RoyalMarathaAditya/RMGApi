using System.ComponentModel.DataAnnotations;

namespace HRMS.Api.Models
{
    public class EmployeeHierarchy
    {
        [Key]
        public int Id { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;

        public int ManagementLeaderId { get; set; }
        public ManagementLeader ManagementLeader { get; set; } = null!;

        public ManagerType ManagerType { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}