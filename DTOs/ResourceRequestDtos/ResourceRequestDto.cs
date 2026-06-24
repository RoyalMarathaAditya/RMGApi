namespace HRMS.Api.DTOs.ResourceRequestDtos
{
    public class ResourceRequestDto
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public int RequestedById { get; set; }
        public string RequestedByName { get; set; } = string.Empty;
        public Guid? PracticeId { get; set; }
        public string? PracticeName { get; set; }
        public string? RequiredSkillIds { get; set; }
        public string? RequiredSkillNames { get; set; }
        public int RequiredCount { get; set; }
        public DateTime RequiredByDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Priority { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? CreatedBy { get; set; }
    }

    public class CreateResourceRequestDto
    {
        public int ProjectId { get; set; }
        public Guid? PracticeId { get; set; }
        public string? RequiredSkillIds { get; set; }
        public int RequiredCount { get; set; } = 1;
        public DateTime RequiredByDate { get; set; }
        public string? Priority { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateResourceRequestDto
    {
        public string? Status { get; set; }
        public string? Notes { get; set; }
    }
}
