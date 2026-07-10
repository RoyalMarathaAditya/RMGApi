namespace HRMS.Api.DTOs.UserDtos
{
    public class UpdateUserDto
    {
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public Guid? RoleId { get; set; }
        public bool? IsActive { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
    }
}