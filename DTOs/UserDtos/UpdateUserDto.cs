namespace HRMS.Api.DTOs.UserDtos
{
    public class UpdateUserDto
    {
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public bool? IsActive { get; set; }
    }
}