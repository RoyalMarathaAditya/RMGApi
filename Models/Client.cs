using System.ComponentModel.DataAnnotations;

namespace HRMS.Api.Models
{
    public class Client
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        public DateTime? ContractStartDate { get; set; }
        public DateTime? ContractEndDate { get; set; }

        public Guid StatusId { get; set; }
        public StatusMaster ClientStatus { get; set; } = null!;

        [MaxLength(200)]
        public string? Location { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string? ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; } = default!;

        public ICollection<Project> Projects { get; set; } = new List<Project>();
    }
}
