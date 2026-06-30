using System.Data;
using System.Globalization;
using HRMS.Api.Data;
using HRMS.Api.DTOs.EmployeeDtos;
using HRMS.Api.Models;
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

        static BulkImportService()
        {
            ExcelPackage.License.SetNonCommercialOrganization("RMG HRMS");
        }

        public BulkImportService(AppDbContext dbContext, IConfiguration configuration, ILogger<BulkImportService> logger)
        {
            _dbContext = dbContext;
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("DefaultConnection string not found");
            _logger = logger;
        }

        public async Task<EmployeeBulkUploadResultDto> ImportAsync(IFormFile file, string? uploadedBy, CancellationToken cancellationToken)
        {
            var result = new EmployeeBulkUploadResultDto();

            var rows = await ParseExcelAsync(file, cancellationToken);
            if (rows.Count == 0)
            {
                result.Success = false;
                result.Errors.Add(new EmployeeImportErrorDto { ErrorMessage = "No data found in the uploaded file." });
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
                    Status = "Completed"
                };
                _dbContext.Set<EmployeeImportHistory>().Add(history);
                await _dbContext.SaveChangesAsync(cancellationToken);

                result.Success = true;
                result.ImportedRows = rows;
            }
            catch (Exception ex)
            {
                try { transaction.Rollback(); } catch { }
                _logger.LogError(ex, "Bulk import failed for batch {BatchId}. Total rows: {TotalRows}", batchId, rows.Count);
                result.Success = false;
                result.TotalRows = rows.Count;
                result.FailedRows = rows.Count;
                result.Errors.Add(new EmployeeImportErrorDto { ErrorMessage = $"Database error: {ex.Message}" });
                return result;
            }

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

        private async Task<List<EmployeeImportRowDto>> ParseExcelAsync(IFormFile file, CancellationToken cancellationToken)
        {
            var rows = new List<EmployeeImportRowDto>();

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream, cancellationToken);
            stream.Position = 0;

            using var package = new ExcelPackage(stream);
            var worksheet = package.Workbook.Worksheets[0];
            if (worksheet.Dimension == null) return rows;

            var headerMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (int col = 1; col <= worksheet.Dimension.Columns; col++)
            {
                var header = worksheet.Cells[1, col].Text?.Trim();
                if (string.IsNullOrEmpty(header)) continue;
                var normalized = System.Text.RegularExpressions.Regex.Replace(header, @"\s*\(.*?\)\s*", " ").Trim();
                if (!string.IsNullOrEmpty(normalized))
                    headerMap[normalized] = col;
            }

            for (int row = 2; row <= worksheet.Dimension.Rows; row++)
            {
                var importRow = new EmployeeImportRowDto
                {
                    RowNumber = row,
                    EmployeeCode = GetString(worksheet, row, headerMap, "Emp Id") ?? GetString(worksheet, row, headerMap, "Employee Code"),
                    FullName = GetString(worksheet, row, headerMap, "Full Name") ?? string.Empty,
                    EmployeeType = GetString(worksheet, row, headerMap, "FTE/ Consultant") ?? GetString(worksheet, row, headerMap, "Employee Type") ?? string.Empty,
                    Designation = GetString(worksheet, row, headerMap, "Role") ?? GetString(worksheet, row, headerMap, "Designation") ?? string.Empty,
                    Practice = GetString(worksheet, row, headerMap, "OU 4 - Practice") ?? GetString(worksheet, row, headerMap, "Practice") ?? string.Empty,
                    SubPractice = GetString(worksheet, row, headerMap, "OU 5 - Sub-practice") ?? GetString(worksheet, row, headerMap, "SubPractice"),
                    NVLocation = GetString(worksheet, row, headerMap, "Location") ?? GetString(worksheet, row, headerMap, "NV Location"),
                    ReportingManager = GetString(worksheet, row, headerMap, "L1 Manager") ?? GetString(worksheet, row, headerMap, "Reporting Manager"),
                    PracticeHead = GetString(worksheet, row, headerMap, "Practice Head"),
                    Email = GetString(worksheet, row, headerMap, "email ID") ?? GetString(worksheet, row, headerMap, "E-mail ID") ?? GetString(worksheet, row, headerMap, "Email") ?? string.Empty,
                    ActiveStatus = GetString(worksheet, row, headerMap, "Active") ?? GetString(worksheet, row, headerMap, "Status"),
                    DOJ = ParseDate(worksheet, row, headerMap, "DOJ"),
                    LWD = ParseDate(worksheet, row, headerMap, "LWD"),
                };

                if (string.IsNullOrWhiteSpace(importRow.EmployeeCode) &&
                    string.IsNullOrWhiteSpace(importRow.FullName) &&
                    string.IsNullOrWhiteSpace(importRow.Email) &&
                    string.IsNullOrWhiteSpace(importRow.Practice) &&
                    string.IsNullOrWhiteSpace(importRow.Designation) &&
                    string.IsNullOrWhiteSpace(importRow.EmployeeType))
                {
                    continue;
                }

                rows.Add(importRow);
            }

            return rows;
        }

        private static string? GetString(ExcelWorksheet ws, int row, Dictionary<string, int> headerMap, string key)
        {
            if (!headerMap.TryGetValue(key, out var col)) return null;
            var value = ws.Cells[row, col].Text?.Trim();
            return string.IsNullOrEmpty(value) ? null : value;
        }

        private static DateTime? ParseDate(ExcelWorksheet ws, int row, Dictionary<string, int> headerMap, string key)
        {
            if (!headerMap.TryGetValue(key, out var col)) return null;
            var cell = ws.Cells[row, col];

            if (cell.Value is DateTime dt)
                return dt;

            if (cell.Value is double oaDate && oaDate > 0)
            {
                try { return DateTime.FromOADate(oaDate); }
                catch { }
            }

            var text = cell.Text?.Trim();
            if (string.IsNullOrWhiteSpace(text)) return null;
            if (DateTime.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
                return parsed;
            return null;
        }

        private static decimal? ParseDecimal(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var num))
                return num;
            return null;
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