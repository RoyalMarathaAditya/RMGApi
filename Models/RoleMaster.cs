namespace HRMS.Api.Models
{
    public class RoleMaster : BaseMasterEntity
    {
        public string? Description { get; set; }
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
