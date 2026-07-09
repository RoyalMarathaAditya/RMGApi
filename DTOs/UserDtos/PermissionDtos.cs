namespace HRMS.Api.DTOs.UserDtos
{
    public class PermissionDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Category { get; set; }
        public bool IsActive { get; set; }
        public bool HasPermission { get; set; }
    }

    public class RolePermissionDto
    {
        public string RoleName { get; set; } = string.Empty;
        public List<int> PermissionIds { get; set; } = new();
    }

    public class RolePermissionsResponseDto
    {
        public string RoleName { get; set; } = string.Empty;
        public List<PermissionDto> Permissions { get; set; } = new();
    }
}