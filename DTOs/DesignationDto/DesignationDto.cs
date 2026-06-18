namespace HRMS.Api.DTOs.DesignationDtos
{
    public class DesignationDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}