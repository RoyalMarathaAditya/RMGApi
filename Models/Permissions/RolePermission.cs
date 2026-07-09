using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.Api.Models.Permissions
{
    public class RolePermission
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string RoleName { get; set; } = string.Empty;

        public int PermissionId { get; set; }

        [ForeignKey("PermissionId")]
        public Permission? Permission { get; set; }
    }
}