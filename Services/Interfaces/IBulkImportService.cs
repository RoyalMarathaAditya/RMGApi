using HRMS.Api.DTOs.EmployeeDtos;

namespace HRMS.Api.Services
{
    public interface IBulkImportService
    {
        Task<EmployeeBulkUploadResultDto> ImportAsync(IFormFile file, string? uploadedBy, CancellationToken cancellationToken = default);
        byte[] GenerateTemplate();
        byte[] GenerateErrorReport(List<EmployeeImportErrorDto> errors);
    }
}
