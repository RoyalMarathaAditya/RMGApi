using System.ComponentModel.DataAnnotations;

namespace HRMS.Api.Models
{
    public class DesignationMaster : BaseMasterEntity
    {
        [MaxLength(20)]
        public string Code { get; set; } = string.Empty;

        public int SortOrder { get; set; }
    }
}
