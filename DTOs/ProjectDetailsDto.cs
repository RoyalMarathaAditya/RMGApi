namespace HRMS.Api.DTOs
{
    public class ProjectDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public int LocationId { get; set; }
        public string LocationName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public List<SkillDto> Skills { get; set; } = new();
    }
}
