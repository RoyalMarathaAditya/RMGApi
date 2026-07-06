namespace HRMS.Api.DTOs
{
    public class ClientDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime? ContractStartDate { get; set; }
        public DateTime? ContractEndDate { get; set; }
        public Guid StatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public string? Location { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string? ModifiedBy { get; set; }
        public string RowVersion { get; set; } = string.Empty;
    }

    public class CreateClientDto
    {
        public string Name { get; set; } = string.Empty;
        public DateTime? ContractStartDate { get; set; }
        public DateTime? ContractEndDate { get; set; }
        public Guid StatusId { get; set; }
        public string? Location { get; set; }
    }

    public class UpdateClientDto
    {
        public string Name { get; set; } = string.Empty;
        public DateTime? ContractStartDate { get; set; }
        public DateTime? ContractEndDate { get; set; }
        public Guid StatusId { get; set; }
        public string? Location { get; set; }
        public string RowVersion { get; set; } = string.Empty;
    }
}
