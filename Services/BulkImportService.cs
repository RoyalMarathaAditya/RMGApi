using System.Data;
using System.Diagnostics;
using System.Globalization;
using HRMS.Api.Data;
using HRMS.Api.DTOs.EmployeeDtos;
using HRMS.Api.Models;
using HRMS.Api.Services.Interfaces.UserManagement;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace HRMS.Api.Services
{
    public class BulkImportService : IBulkImportService
    {
        private readonly AppDbContext _dbContext;
        private readonly string _connectionString;
        private readonly ILogger<BulkImportService> _logger;
        private readonly DynamicExcelMapper _dynamicMapper;
        private readonly IUserSynchronizationService _userSyncService;
        private List<UploadColumnInfo>? _lastUploadedColumns;

        static BulkImportService()
        {
            ExcelPackage.License.SetNonCommercialOrganization("RMG HRMS");
        }

        public BulkImportService(AppDbContext dbContext, IConfiguration configuration, ILogger<BulkImportService> logger, DynamicExcelMapper dynamicMapper, IUserSynchronizationService userSyncService)
        {
            _dbContext = dbContext;
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("DefaultConnection string not found");
            _logger = logger;
            _dynamicMapper = dynamicMapper;
            _userSyncService = userSyncService;
        }

        public async Task<EmployeeBulkUploadResultDto> ImportAsync(IFormFile file, string? uploadedBy, CancellationToken cancellationToken = default)
        {
            var sw = Stopwatch.StartNew();
            var result = new EmployeeBulkUploadResultDto();

            _logger.LogInformation("Starting bulk import. File: {FileName}, UploadedBy: {UploadedBy}",
                file.FileName, uploadedBy);

            var (rows, columnErrors) = await ParseExcelAsync(file, cancellationToken);

            if (columnErrors.Any())
            {
                result.Success = false;
                result.FailedRows = columnErrors.Count;
                result.Errors = columnErrors.Select(e => new EmployeeImportErrorDto
                {
                    RowNumber = 0,
                    ErrorMessage = e
                }).ToList();
                sw.Stop();
                _logger.LogInformation("Bulk import completed (column errors). File: {FileName}, Elapsed: {ElapsedMs}ms, Result: {Success}",
                    file.FileName, sw.ElapsedMilliseconds, result.Success);
                return result;
            }

            if (rows.Count == 0)
            {
                result.Success = false;
                result.Errors.Add(new EmployeeImportErrorDto { ErrorMessage = "No data found in the uploaded file." });
                sw.Stop();
                _logger.LogInformation("Bulk import completed (no data). File: {FileName}, Elapsed: {ElapsedMs}ms, Result: {Success}",
                    file.FileName, sw.ElapsedMilliseconds, result.Success);
                return result;
            }

            result.TotalRows = rows.Count;

            var errors = await ValidateRowsAsync(rows, cancellationToken);
            if (errors.Any())
            {
                result.Success = false;
                result.FailedRows = errors.Count;
                result.SuccessRows = result.TotalRows - result.FailedRows;
                result.Errors = errors;
                result.ErrorFileUrl = await SaveErrorReportAsync(errors, cancellationToken);
                result.ImportedRows = rows;
                sw.Stop();
                _logger.LogInformation("Bulk import completed (validation errors). File: {FileName}, Elapsed: {ElapsedMs}ms, TotalRows: {TotalRows}, FailedRows: {FailedRows}",
                    file.FileName, sw.ElapsedMilliseconds, result.TotalRows, result.FailedRows);
                return result;
            }

            await AutoGenerateEmployeeCodesAsync(rows, cancellationToken);

            var batchId = Guid.NewGuid();

            var dataTable = ConvertToDataTable(rows, batchId, uploadedBy);

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            using var transaction = connection.BeginTransaction();

            try
            {
                using var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction)
                {
                    DestinationTableName = "EmployeeImportStaging",
                    BatchSize = 5000,
                    BulkCopyTimeout = 300
                };

                bulkCopy.ColumnMappings.Add("BatchId", "BatchId");
                bulkCopy.ColumnMappings.Add("EmployeeCode", "EmployeeCode");
                bulkCopy.ColumnMappings.Add("FullName", "FullName");
                bulkCopy.ColumnMappings.Add("Email", "Email");
                bulkCopy.ColumnMappings.Add("EmployeeType", "EmployeeType");
                bulkCopy.ColumnMappings.Add("Designation", "Designation");
                bulkCopy.ColumnMappings.Add("Practice", "Practice");
                bulkCopy.ColumnMappings.Add("SubPractice", "SubPractice");
                bulkCopy.ColumnMappings.Add("Location", "Location");
                bulkCopy.ColumnMappings.Add("ReportingManager", "ReportingManager");
                bulkCopy.ColumnMappings.Add("PracticeHead", "PracticeHead");
                bulkCopy.ColumnMappings.Add("ActiveStatus", "ActiveStatus");
                bulkCopy.ColumnMappings.Add("DOJ", "DOJ");
                bulkCopy.ColumnMappings.Add("LWD", "LWD");
                bulkCopy.ColumnMappings.Add("ImportedOn", "ImportedOn");
                bulkCopy.ColumnMappings.Add("ImportedBy", "ImportedBy");

                await bulkCopy.WriteToServerAsync(dataTable, cancellationToken);

                using var clearRefCmd = new SqlCommand("UPDATE Users SET EmployeeId = NULL WHERE EmployeeId IS NOT NULL", connection, transaction)
                {
                    CommandTimeout = 120
                };
                await clearRefCmd.ExecuteNonQueryAsync(cancellationToken);

                using var cmd = new SqlCommand("sp_ProcessEmployeeImport", connection, transaction)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 300
                };
                cmd.Parameters.AddWithValue("@BatchId", batchId);
                cmd.Parameters.AddWithValue("@ImportedBy", (object?)uploadedBy ?? DBNull.Value);

                using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
                if (await reader.ReadAsync(cancellationToken))
                {
                    result.TotalRows = reader.GetInt32(reader.GetOrdinal("TotalRows"));
                    result.SuccessRows = reader.GetInt32(reader.GetOrdinal("ImportedRows"));
                    result.FailedRows = reader.GetInt32(reader.GetOrdinal("FailedRows"));
                    result.DeletedRows = reader.GetInt32(reader.GetOrdinal("DeletedRows"));
                }
                await reader.CloseAsync();

                if (result.FailedRows > 0)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    result.Success = false;
                    sw.Stop();
                    _logger.LogInformation("Bulk import completed (SP failed rows). File: {FileName}, Elapsed: {ElapsedMs}ms, FailedRows: {FailedRows}",
                        file.FileName, sw.ElapsedMilliseconds, result.FailedRows);
                    return result;
                }

                transaction.Commit();

                var history = new EmployeeImportHistory
                {
                    BatchId = batchId,
                    FileName = file.FileName,
                    ImportedBy = uploadedBy,
                    ImportedOn = DateTime.UtcNow,
                    TotalRows = result.TotalRows,
                    ImportedRows = result.SuccessRows,
                    FailedRows = result.FailedRows,
                    DeletedRows = result.DeletedRows,
                    Status = "Completed",
                    UploadedColumns = _lastUploadedColumns != null ? System.Text.Json.JsonSerializer.Serialize(_lastUploadedColumns) : null
                };
                _dbContext.Set<EmployeeImportHistory>().Add(history);
                await _dbContext.SaveChangesAsync(cancellationToken);

                try
                {
                    await _userSyncService.SyncEmployeesAsync(cancellationToken);
                }
                catch (Exception syncEx)
                {
                    _logger.LogError(syncEx, "User synchronization failed after bulk import for batch {BatchId}, but employee import was successful", batchId);
                }

                result.Columns = _lastUploadedColumns;

                result.Success = true;
                result.ImportedRows = rows;
            }
            catch (Exception ex)
            {
                try { transaction.Rollback(); } catch { }
                sw.Stop();
                _logger.LogError(ex, "Bulk import failed for batch {BatchId}. File: {FileName}, Elapsed: {ElapsedMs}ms, Total rows: {TotalRows}", batchId, file.FileName, sw.ElapsedMilliseconds, rows.Count);
                result.Success = false;
                result.TotalRows = rows.Count;
                result.FailedRows = rows.Count;
                result.Errors.Add(new EmployeeImportErrorDto { ErrorMessage = $"Database error: {ex.Message}" });
                return result;
            }

            sw.Stop();
            _logger.LogInformation("Bulk import completed successfully. File: {FileName}, Elapsed: {ElapsedMs}ms, TotalRows: {TotalRows}, SuccessRows: {SuccessRows}, DeletedRows: {DeletedRows}",
                file.FileName, sw.ElapsedMilliseconds, result.TotalRows, result.SuccessRows, result.DeletedRows);
            return result;
        }

        public byte[] GenerateTemplate()
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Template");

            var headers = new[]
            {
                "Emp Id", "Full Name", "FTE/ Consultant", "Role",
                "OU 4 - Practice", "OU 5 - Sub-practice", "Location",
                "L1 Manager", "Practice Head", "email ID",
                "Active", "DOJ", "LWD"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = headers[i];
                worksheet.Cells[1, i + 1].Style.Font.Bold = true;
            }

            worksheet.Cells.AutoFitColumns();
            return package.GetAsByteArray();
        }

        public byte[] GenerateErrorReport(List<EmployeeImportErrorDto> errors)
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Errors");

            worksheet.Cells[1, 1].Value = "Row Number";
            worksheet.Cells[1, 2].Value = "Employee Name";
            worksheet.Cells[1, 3].Value = "Email";
            worksheet.Cells[1, 4].Value = "Error Message";

            for (int i = 0; i < 4; i++)
                worksheet.Cells[1, i + 1].Style.Font.Bold = true;

            for (int i = 0; i < errors.Count; i++)
            {
                var error = errors[i];
                worksheet.Cells[i + 2, 1].Value = error.RowNumber;
                worksheet.Cells[i + 2, 2].Value = error.EmployeeName;
                worksheet.Cells[i + 2, 3].Value = error.Email;
                worksheet.Cells[i + 2, 4].Value = error.ErrorMessage;
            }

            worksheet.Cells.AutoFitColumns();
            return package.GetAsByteArray();
        }

        private async Task<(List<EmployeeImportRowDto> Rows, List<string> Errors)> ParseExcelAsync(IFormFile file, CancellationToken cancellationToken)
        {
            using var stream = new MemoryStream();
            await file.CopyToAsync(stream, cancellationToken);
            stream.Position = 0;

            using var package = new ExcelPackage(stream);
            var worksheet = package.Workbook.Worksheets[0];
            if (worksheet.Dimension == null) return (new List<EmployeeImportRowDto>(), new List<string>());

            var (dynamicRows, uploadedColumns, warnings, errors) = await _dynamicMapper.MapExcelToDtoAsync(worksheet, cancellationToken);

            foreach (var warning in warnings)
            {
                _logger.LogWarning("Dynamic mapping: {Warning}", warning);
            }

            _lastUploadedColumns = uploadedColumns.Count > 0 ? uploadedColumns : null;

            if (errors.Any())
            {
                return (dynamicRows, errors);
            }

            return (dynamicRows, new List<string>());
        }

        private async Task<List<EmployeeImportErrorDto>> ValidateRowsAsync(List<EmployeeImportRowDto> rows, CancellationToken cancellationToken)
        {
            var errors = new List<EmployeeImportErrorDto>();
            var validator = new Validators.EmployeeImportRowValidator();
            var emailSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var employeeCodeSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var row in rows)
            {
                var rowErrors = new List<string>();

                var validationResult = await validator.ValidateAsync(row, cancellationToken);
                rowErrors.AddRange(validationResult.Errors.Select(e => e.ErrorMessage));

                if (!string.IsNullOrWhiteSpace(row.EmployeeCode))
                {
                    if (!employeeCodeSet.Add(row.EmployeeCode))
                        rowErrors.Add("Duplicate employee code found within the uploaded file.");
                }

                if (!string.IsNullOrEmpty(row.Email))
                {
                    if (!emailSet.Add(row.Email))
                        rowErrors.Add("Duplicate email found within the uploaded file.");
                }

                if (rowErrors.Any())
                {
                    errors.Add(new EmployeeImportErrorDto
                    {
                        RowNumber = row.RowNumber,
                        EmployeeName = row.FullName,
                        Email = row.Email,
                        ErrorMessage = string.Join("; ", rowErrors)
                    });
                }
            }

            return errors;
        }

        private async Task<string> SaveErrorReportAsync(List<EmployeeImportErrorDto> errors, CancellationToken cancellationToken)
        {
            var bytes = GenerateErrorReport(errors);
            var errorDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "errors");
            Directory.CreateDirectory(errorDir);
            var fileName = $"ImportErrors_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx";
            var filePath = Path.Combine(errorDir, fileName);
            await System.IO.File.WriteAllBytesAsync(filePath, bytes, cancellationToken);
            return $"/uploads/errors/{fileName}";
        }

        private async Task AutoGenerateEmployeeCodesAsync(List<EmployeeImportRowDto> rows, CancellationToken cancellationToken)
        {
            var autoCodeCounter = await GetNextEmployeeCodeNumberAsync(cancellationToken);
            foreach (var row in rows)
            {
                if (string.IsNullOrWhiteSpace(row.EmployeeCode))
                {
                    row.EmployeeCode = $"EMP{autoCodeCounter:D4}";
                    autoCodeCounter++;
                }
            }
        }

        private async Task<int> GetNextEmployeeCodeNumberAsync(CancellationToken cancellationToken)
        {
            var lastCode = await _dbContext.Employees
                .IgnoreQueryFilters()
                .OrderByDescending(e => e.Id)
                .Select(e => e.EmployeeCode)
                .FirstOrDefaultAsync(cancellationToken);

            int nextNumber = 1;
            if (!string.IsNullOrEmpty(lastCode) && lastCode.StartsWith("EMP"))
            {
                int.TryParse(lastCode[3..], out nextNumber);
                nextNumber++;
            }
            else
            {
                var maxExisting = await _dbContext.Employees
                    .IgnoreQueryFilters()
                    .Where(e => e.EmployeeCode.StartsWith("EMP"))
                    .OrderByDescending(e => e.EmployeeCode)
                    .Select(e => e.EmployeeCode)
                    .FirstOrDefaultAsync(cancellationToken);

                if (!string.IsNullOrEmpty(maxExisting))
                {
                    int.TryParse(maxExisting[3..], out nextNumber);
                    nextNumber++;
                }
            }

            return nextNumber;
        }

        private static DataTable ConvertToDataTable(List<EmployeeImportRowDto> rows, Guid batchId, string? uploadedBy)
        {
            var table = new DataTable();

            table.Columns.Add("BatchId", typeof(Guid));
            table.Columns.Add("EmployeeCode", typeof(string));
            table.Columns.Add("FullName", typeof(string));
            table.Columns.Add("Email", typeof(string));
            table.Columns.Add("EmployeeType", typeof(string));
            table.Columns.Add("Designation", typeof(string));
            table.Columns.Add("Practice", typeof(string));
            table.Columns.Add("SubPractice", typeof(string));
            table.Columns.Add("Location", typeof(string));
            table.Columns.Add("ReportingManager", typeof(string));
            table.Columns.Add("PracticeHead", typeof(string));
            table.Columns.Add("ActiveStatus", typeof(string));
            table.Columns.Add("DOJ", typeof(DateTime));
            table.Columns.Add("LWD", typeof(DateTime));
            table.Columns.Add("ImportedOn", typeof(DateTime));
            table.Columns.Add("ImportedBy", typeof(string));

            var now = DateTime.UtcNow;

            foreach (var row in rows)
            {
                var dr = table.NewRow();
                dr["BatchId"] = batchId;
                dr["EmployeeCode"] = (object?)row.EmployeeCode ?? DBNull.Value;
                dr["FullName"] = (object?)row.FullName ?? DBNull.Value;
                dr["Email"] = (object?)row.Email ?? DBNull.Value;
                dr["EmployeeType"] = (object?)row.EmployeeType ?? DBNull.Value;
                dr["Designation"] = (object?)row.Designation ?? DBNull.Value;
                dr["Practice"] = (object?)row.Practice ?? DBNull.Value;
                dr["SubPractice"] = (object?)row.SubPractice ?? DBNull.Value;
                dr["Location"] = (object?)row.NVLocation ?? DBNull.Value;
                dr["ReportingManager"] = (object?)row.ReportingManager ?? DBNull.Value;
                dr["PracticeHead"] = (object?)row.PracticeHead ?? DBNull.Value;
                dr["ActiveStatus"] = (object?)row.ActiveStatus ?? DBNull.Value;
                dr["DOJ"] = (object?)row.DOJ ?? DBNull.Value;
                dr["LWD"] = (object?)row.LWD ?? DBNull.Value;
                dr["ImportedOn"] = now;
                dr["ImportedBy"] = (object?)uploadedBy ?? DBNull.Value;
                table.Rows.Add(dr);
            }

            return table;
        }
    }
}