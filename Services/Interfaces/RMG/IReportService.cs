using HRMS.Api.DTOs.ReportDtos;

namespace HRMS.Api.Services.Interfaces.RMG
{
    public interface IReportService
    {
        Task<IEnumerable<PracticeWiseReportDto>> GetPracticeWiseReportAsync(CancellationToken cancellationToken = default);
        Task<byte[]> ExportPracticeWiseReportAsync(CancellationToken cancellationToken = default);
    }
}
