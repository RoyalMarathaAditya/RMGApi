using HRMS.Api.DTOs.ReportDtos;

namespace HRMS.Api.Services.Interfaces.RMG
{
    public interface IReportService
    {
        Task<IEnumerable<PracticeWiseReportDto>> GetPracticeWiseReportAsync(bool engineeringOnly = false, CancellationToken cancellationToken = default);
        Task<byte[]> ExportPracticeWiseReportAsync(bool engineeringOnly = false, CancellationToken cancellationToken = default);

        Task<IEnumerable<ClientWiseReportDto>> GetClientWiseReportAsync(string? clientIds, string? statusFilter, CancellationToken cancellationToken = default);
        Task<ReportChartDataDto> GetClientWiseChartDataAsync(CancellationToken cancellationToken = default);

        Task<IEnumerable<ProjectWiseReportDto>> GetProjectWiseReportAsync(string? clientFilter, string? practiceFilter, string? statusFilter, string? searchFilter, CancellationToken cancellationToken = default);
        Task<ReportChartDataDto> GetProjectWiseChartDataAsync(CancellationToken cancellationToken = default);
    }
}
