namespace HRMS.Api.Services
{
    public interface IExcelExportService
    {
        Task<byte[]> ExportEmployeesAsync(string? fullName = null, Guid? practiceId = null, DateTime? doj = null, Guid? statusId = null, CancellationToken cancellationToken = default);
    }
}