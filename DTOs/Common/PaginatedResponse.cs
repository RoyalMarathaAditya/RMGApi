namespace HRMS.Api.DTOs.Common
{
    public class PaginatedResponse<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / Math.Max(1, PageSize));
    }

    public class PagedFilterDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 25;
        public string? SortField { get; set; }
        public string? SortDirection { get; set; } = "asc";
    }
}
