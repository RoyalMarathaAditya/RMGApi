using System.ComponentModel.DataAnnotations;

namespace HRMS.Api.Models
{
    public class EmployeeImportHistory
    {
        [Key]
        public Guid BatchId { get; set; }

        [Required]
        [MaxLength(255)]
        public string FileName { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? ImportedBy { get; set; }

        public DateTime ImportedOn { get; set; } = DateTime.UtcNow;

        public int TotalRows { get; set; }

        public int ImportedRows { get; set; }

        public int FailedRows { get; set; }

        public int DeletedRows { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "Completed";

        public string? UploadedColumns { get; set; }
    }
}