namespace HRMS.Api.DTOs.UserDtos
{
    public class CreateUserDto
    {
        public int? EmployeeId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public Guid RoleId { get; set; }
        public bool IsActive { get; set; } = true;
    }
}