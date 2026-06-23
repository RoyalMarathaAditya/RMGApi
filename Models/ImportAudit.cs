using System.ComponentModel.DataAnnotations;

namespace HRMS.Api.Models
{
    public class ImportAudit
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string FileName { get; set; } = string.Empty;

        public int TotalRows { get; set; }
        public int SuccessRows { get; set; }
        public int FailedRows { get; set; }

        [MaxLength(100)]
        public string? UploadedBy { get; set; }

        public DateTime UploadedOn { get; set; } = DateTime.UtcNow;
    }
}
