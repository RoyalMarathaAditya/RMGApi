namespace HRMS.Api.DTOs.Common
{
    public class PaginationParams
    {
        private const int MaxPageSize = 100;
        public int PageNumber { get; set; } = 1;
        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }
        public string? SearchTerm { get; set; }
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; }
        public string? RoleIdFilter { get; set; }
        public string? StatusFilter { get; set; }
        public string? PracticeFilter { get; set; }
        public string? DepartmentFilter { get; set; }
        public string? DesignationFilter { get; set; }
    }
}
