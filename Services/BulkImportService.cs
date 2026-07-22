using System.Collections.Concurrent;
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

        private static readonly ConcurrentDictionary<string, System.Reflection.PropertyInfo?> EmployeePropertyCache = new();
        private static readonly Type EmployeeEntityType = typeof(Employee);
        private static readonly HashSet<Type> SupportedTypes = new()
        {
            typeof(string), typeof(int), typeof(long), typeof(decimal), typeof(double), typeof(float),
            typeof(bool), typeof(DateTime), typeof(Guid),
            typeof(int?), typeof(long?), typeof(decimal?), typeof(double?), typeof(float?),
            typeof(bool?), typeof(DateTime?), typeof(Guid?)
        };

        private static readonly HashSet<string> FixedColumnProperties = new(StringComparer.OrdinalIgnoreCase)
        {
            "EmployeeCode", "FullName", "Email", "DOJ", "LWD",
            "DesignationId", "EmploymentTypeId", "LocationId", "PracticeId", "SubPracticeId",
            "StatusId", "ReportingManagerName", "PracticeHeadName", "AdditionalData",
            "EmployeeTypeId", "ReportingManagerId", "PracticeHeadId"
        };

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
                bulkCopy.ColumnMappings.Add("AdditionalData", "AdditionalData");
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

                try
                {
                    await DistributeAdditionalDataAsync(cancellationToken);
                }
                catch (Exception distEx)
                {
                    _logger.LogError(distEx, "AdditionalData distribution failed for batch {BatchId}. Values remain in AdditionalData column.", batchId);
                }

                result.Columns = _lastUploadedColumns;

                result.Success = true;
                result.ImportedRows = rows;

                sw.Stop();
                _logger.LogInformation("Bulk import completed successfully. File: {FileName}, Elapsed: {ElapsedMs}ms, TotalRows: {TotalRows}, ImportedRows: {ImportedRows}, DeletedRows: {DeletedRows}",
                    file.FileName, sw.ElapsedMilliseconds, result.TotalRows, result.SuccessRows, result.DeletedRows);
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

        public async Task<EmployeeBulkUploadPreviewDto> PreviewAsync(IFormFile file, CancellationToken cancellationToken = default)
        {
            var result = new EmployeeBulkUploadPreviewDto();

            var (rows, columnErrors) = await ParseExcelAsync(file, cancellationToken);

            if (columnErrors.Any())
            {
                result.Success = false;
                result.ErrorMessage = string.Join("; ", columnErrors);
                return result;
            }

            if (rows.Count == 0)
            {
                result.Success = false;
                result.ErrorMessage = "No data found in the uploaded file.";
                return result;
            }

            result.TotalRows = rows.Count;

            var validationErrors = await ValidateRowsAsync(rows, cancellationToken);
            if (validationErrors.Any())
            {
                result.Success = false;
                result.ErrorMessage = "Validation failed. Please fix errors and try again.";
                return result;
            }

            await AutoGenerateEmployeeCodesAsync(rows, cancellationToken);

            var importedEmails = rows.Select(r => r.Email?.Trim().ToLowerInvariant() ?? "")
                .Where(e => !string.IsNullOrEmpty(e))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var allDbEmails = await _dbContext.Employees
                .IgnoreQueryFilters()
                .Where(e => !e.IsDeleted)
                .Select(e => e.Email)
                .ToListAsync(cancellationToken);

            var dbEmailSet = allDbEmails
                .Select(e => e?.Trim().ToLowerInvariant() ?? "")
                .Where(e => !string.IsNullOrEmpty(e))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var matchingEmails = importedEmails.Where(e => dbEmailSet.Contains(e)).ToHashSet(StringComparer.OrdinalIgnoreCase);

            var newEmails = importedEmails.Except(matchingEmails, StringComparer.OrdinalIgnoreCase).ToList();
            var deletedEmails = dbEmailSet.Except(importedEmails, StringComparer.OrdinalIgnoreCase).ToList();

            var matchingEmployees = new List<Employee>();
            if (matchingEmails.Any())
            {
                matchingEmployees = await _dbContext.Employees
                    .IgnoreQueryFilters()
                    .Where(e => !e.IsDeleted && matchingEmails.Contains(e.Email))
                    .Include(e => e.Designation)
                    .Include(e => e.EmploymentType)
                    .Include(e => e.Practice)
                    .Include(e => e.SubPractice)
                    .Include(e => e.Location)
                    .Include(e => e.EmployeeStatus)
                    .ToListAsync(cancellationToken);
            }

            var employeeByEmail = matchingEmployees
                .GroupBy(e => e.Email?.Trim().ToLowerInvariant() ?? "")
                .ToDictionary(g => g.Key, g => g.First());

            var columnMappings = await _dbContext.Set<Models.ColumnMapping>()
                .AsNoTracking()
                .Where(m => m.EntityType == "employee-import" && m.IsActive)
                .ToListAsync(cancellationToken);

            var displayNameMap = columnMappings
                .GroupBy(m => m.TargetProperty, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First().TargetDisplayName ?? g.Key, StringComparer.OrdinalIgnoreCase);

            var changes = new List<EmployeeChangeDto>();

            foreach (var row in rows)
            {
                var emailKey = row.Email?.Trim().ToLowerInvariant() ?? "";
                if (string.IsNullOrEmpty(emailKey) || !employeeByEmail.TryGetValue(emailKey, out var existing))
                    continue;

                var fieldChanges = ComputeFieldChanges(existing, row, displayNameMap);
                if (fieldChanges.Any())
                {
                    changes.Add(new EmployeeChangeDto
                    {
                        Email = row.Email ?? "",
                        EmployeeCode = row.EmployeeCode ?? existing.EmployeeCode,
                        FullName = row.FullName,
                        FieldChanges = fieldChanges
                    });
                }
            }

            var newEmployeeInfos = rows
                .Where(r => newEmails.Contains(r.Email?.Trim().ToLowerInvariant() ?? ""))
                .Select(r => new SimpleEmployeeInfo { Email = r.Email, FullName = r.FullName })
                .ToList();

            var deletedEmployeeInfos = new List<SimpleEmployeeInfo>();
            if (deletedEmails.Any())
            {
                deletedEmployeeInfos = await _dbContext.Employees
                    .IgnoreQueryFilters()
                    .Where(e => !e.IsDeleted && deletedEmails.Contains(e.Email))
                    .Select(e => new SimpleEmployeeInfo { Email = e.Email, FullName = e.FullName })
                    .ToListAsync(cancellationToken);
            }

            result.TotalRows = rows.Count;
            result.NewEmployees = newEmployeeInfos.Count;
            result.UpdatedEmployees = changes.Count;
            result.DeletedEmployees = deletedEmployeeInfos.Count;
            result.Changes = changes;
            result.NewEmployeeList = newEmployeeInfos;
            result.DeletedEmployeeList = deletedEmployeeInfos;

            return result;
        }

        private List<FieldChangeDto> ComputeFieldChanges(Employee existing, EmployeeImportRowDto row, Dictionary<string, string> displayNameMap)
        {
            var changes = new List<FieldChangeDto>();

            var oldFullName = existing.FullName ?? "";
            var newFullName = row.FullName ?? "";
            if (!string.Equals(oldFullName.Trim(), newFullName.Trim(), StringComparison.OrdinalIgnoreCase))
                changes.Add(new FieldChangeDto
                {
                    FieldName = GetDisplayName(displayNameMap, nameof(EmployeeImportRowDto.FullName), "Full Name"),
                    OldValue = oldFullName,
                    NewValue = newFullName
                });

            var oldEmployeeCode = existing.EmployeeCode ?? "";
            var newEmployeeCode = row.EmployeeCode ?? "";
            if (!string.Equals(oldEmployeeCode.Trim(), newEmployeeCode.Trim(), StringComparison.OrdinalIgnoreCase))
                changes.Add(new FieldChangeDto
                {
                    FieldName = GetDisplayName(displayNameMap, nameof(EmployeeImportRowDto.EmployeeCode), "Emp Id"),
                    OldValue = oldEmployeeCode,
                    NewValue = newEmployeeCode
                });

            var oldEmail = existing.Email ?? "";
            var newEmail = row.Email ?? "";
            if (!string.Equals(oldEmail.Trim(), newEmail.Trim(), StringComparison.OrdinalIgnoreCase))
                changes.Add(new FieldChangeDto
                {
                    FieldName = GetDisplayName(displayNameMap, nameof(EmployeeImportRowDto.Email), "email ID"),
                    OldValue = oldEmail,
                    NewValue = newEmail
                });

            var oldDesignation = existing.Designation?.Name ?? "";
            var newDesignation = row.Designation ?? "";
            if (!string.Equals(oldDesignation.Trim(), newDesignation.Trim(), StringComparison.OrdinalIgnoreCase))
                changes.Add(new FieldChangeDto
                {
                    FieldName = GetDisplayName(displayNameMap, nameof(EmployeeImportRowDto.Designation), "Role"),
                    OldValue = oldDesignation,
                    NewValue = newDesignation
                });

            var oldEmployeeType = existing.EmploymentType?.Name ?? "";
            var newEmployeeType = row.EmployeeType ?? "";
            if (!string.Equals(oldEmployeeType.Trim(), newEmployeeType.Trim(), StringComparison.OrdinalIgnoreCase))
                changes.Add(new FieldChangeDto
                {
                    FieldName = GetDisplayName(displayNameMap, nameof(EmployeeImportRowDto.EmployeeType), "FTE/ Consultant"),
                    OldValue = oldEmployeeType,
                    NewValue = newEmployeeType
                });

            var oldPractice = existing.Practice?.Name ?? "";
            var newPractice = row.Practice ?? "";
            if (!string.Equals(oldPractice.Trim(), newPractice.Trim(), StringComparison.OrdinalIgnoreCase))
                changes.Add(new FieldChangeDto
                {
                    FieldName = GetDisplayName(displayNameMap, nameof(EmployeeImportRowDto.Practice), "OU 4 - Practice"),
                    OldValue = oldPractice,
                    NewValue = newPractice
                });

            var oldSubPractice = existing.SubPractice?.Name ?? "";
            var newSubPractice = row.SubPractice ?? "";
            if (!string.Equals(oldSubPractice.Trim(), newSubPractice.Trim(), StringComparison.OrdinalIgnoreCase))
                changes.Add(new FieldChangeDto
                {
                    FieldName = GetDisplayName(displayNameMap, nameof(EmployeeImportRowDto.SubPractice), "OU 5 - Sub-practice"),
                    OldValue = string.IsNullOrEmpty(oldSubPractice) ? null : oldSubPractice,
                    NewValue = string.IsNullOrEmpty(newSubPractice) ? null : newSubPractice
                });

            var oldLocation = existing.Location?.Name ?? "";
            var newLocation = row.NVLocation ?? "";
            if (!string.Equals(oldLocation.Trim(), newLocation.Trim(), StringComparison.OrdinalIgnoreCase))
                changes.Add(new FieldChangeDto
                {
                    FieldName = GetDisplayName(displayNameMap, nameof(EmployeeImportRowDto.NVLocation), "Location"),
                    OldValue = string.IsNullOrEmpty(oldLocation) ? null : oldLocation,
                    NewValue = string.IsNullOrEmpty(newLocation) ? null : newLocation
                });

            var oldReportingManager = existing.ReportingManagerName ?? "";
            var newReportingManager = row.ReportingManager ?? "";
            if (!string.Equals(oldReportingManager.Trim(), newReportingManager.Trim(), StringComparison.OrdinalIgnoreCase))
                changes.Add(new FieldChangeDto
                {
                    FieldName = GetDisplayName(displayNameMap, nameof(EmployeeImportRowDto.ReportingManager), "L1 Manager"),
                    OldValue = string.IsNullOrEmpty(oldReportingManager) ? null : oldReportingManager,
                    NewValue = string.IsNullOrEmpty(newReportingManager) ? null : newReportingManager
                });

            var oldPracticeHead = existing.PracticeHeadName ?? "";
            var newPracticeHead = row.PracticeHead ?? "";
            if (!string.Equals(oldPracticeHead.Trim(), newPracticeHead.Trim(), StringComparison.OrdinalIgnoreCase))
                changes.Add(new FieldChangeDto
                {
                    FieldName = GetDisplayName(displayNameMap, nameof(EmployeeImportRowDto.PracticeHead), "Practice Head"),
                    OldValue = string.IsNullOrEmpty(oldPracticeHead) ? null : oldPracticeHead,
                    NewValue = string.IsNullOrEmpty(newPracticeHead) ? null : newPracticeHead
                });

            var oldActiveStatus = existing.EmployeeStatus?.Name ?? "";
            var newActiveStatus = row.ActiveStatus ?? "";
            if (!string.Equals(oldActiveStatus.Trim(), newActiveStatus.Trim(), StringComparison.OrdinalIgnoreCase))
                changes.Add(new FieldChangeDto
                {
                    FieldName = GetDisplayName(displayNameMap, nameof(EmployeeImportRowDto.ActiveStatus), "Active"),
                    OldValue = string.IsNullOrEmpty(oldActiveStatus) ? null : oldActiveStatus,
                    NewValue = string.IsNullOrEmpty(newActiveStatus) ? null : newActiveStatus
                });

            var oldDoj = existing.DOJ;
            if (row.DOJ.HasValue && oldDoj.Date != row.DOJ.Value.Date)
                changes.Add(new FieldChangeDto
                {
                    FieldName = GetDisplayName(displayNameMap, nameof(EmployeeImportRowDto.DOJ), "DOJ"),
                    OldValue = oldDoj.ToString("yyyy-MM-dd"),
                    NewValue = row.DOJ.Value.ToString("yyyy-MM-dd")
                });

            var oldLwd = existing.LWD;
            if (row.LWD.HasValue && oldLwd?.Date != row.LWD.Value.Date)
                changes.Add(new FieldChangeDto
                {
                    FieldName = GetDisplayName(displayNameMap, nameof(EmployeeImportRowDto.LWD), "LWD"),
                    OldValue = oldLwd?.ToString("yyyy-MM-dd"),
                    NewValue = row.LWD.Value.ToString("yyyy-MM-dd")
                });

            var existingAdditionalData = DeserializeAdditionalData(existing.AdditionalData);
            _logger.LogDebug("DIAG ComputeFieldChanges: row.AdditionalFields keys=[{Keys}], existingAdditionalData keys=[{ExistKeys}], existing.AdditionalData=[{Ad}]",
                string.Join(",", row.AdditionalFields.Keys), string.Join(",", existingAdditionalData.Keys), existing.AdditionalData);
            foreach (var kvp in row.AdditionalFields)
            {
                string? oldVal;
                var employeeProp = EmployeePropertyCache.GetOrAdd(kvp.Key, _ =>
                    EmployeeEntityType.GetProperty(kvp.Key,
                        System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase));
                if (employeeProp != null && employeeProp.CanRead)
                {
                    var rawValue = employeeProp.GetValue(existing);
                    oldVal = rawValue?.ToString() ?? "";
                    var normalizedNew = ConvertImportValue(kvp.Value, employeeProp.PropertyType);
                    var newValStr = normalizedNew?.ToString() ?? "";
                    _logger.LogDebug("DIAG ComputeFieldChanges key='{Key}': employeeProp found, rawValue={Raw}, oldVal={Old}, convertedNew={Conv}, newValStr={New}, equal={Eq}",
                        kvp.Key, rawValue, oldVal, normalizedNew, newValStr,
                        string.Equals(oldVal, newValStr, StringComparison.OrdinalIgnoreCase));
                    if (!string.Equals(oldVal, newValStr, StringComparison.OrdinalIgnoreCase))
                    {
                        changes.Add(new FieldChangeDto
                        {
                            FieldName = displayNameMap.TryGetValue(kvp.Key, out var dn) ? dn : kvp.Key,
                            OldValue = oldVal,
                            NewValue = kvp.Value
                        });
                        _logger.LogDebug("DIAG ComputeFieldChanges: ADDED change for '{Key}': '{Old}' -> '{New}'", kvp.Key, oldVal, kvp.Value);
                    }
                }
                else
                {
                    oldVal = existingAdditionalData.TryGetValue(kvp.Key, out var existingVal) ? existingVal : null;
                    _logger.LogDebug("DIAG ComputeFieldChanges key='{Key}': employeeProp null/!CanRead, oldVal={Old}, kvp.Value={New}, equal={Eq}",
                        kvp.Key, oldVal, kvp.Value,
                        string.Equals(oldVal ?? "", kvp.Value ?? "", StringComparison.OrdinalIgnoreCase));
                    if (!string.Equals(oldVal ?? "", kvp.Value ?? "", StringComparison.OrdinalIgnoreCase))
                    {
                        changes.Add(new FieldChangeDto
                        {
                            FieldName = displayNameMap.TryGetValue(kvp.Key, out var dn) ? dn : kvp.Key,
                            OldValue = oldVal,
                            NewValue = kvp.Value
                        });
                        _logger.LogDebug("DIAG ComputeFieldChanges: ADDED change (else) for '{Key}': '{Old}' -> '{New}'", kvp.Key, oldVal, kvp.Value);
                    }
                }
            }

            foreach (var kvp in existingAdditionalData)
            {
                if (row.AdditionalFields.ContainsKey(kvp.Key)) continue;

                string? currentVal;
                var employeeProp = EmployeePropertyCache.GetOrAdd(kvp.Key, _ =>
                    EmployeeEntityType.GetProperty(kvp.Key,
                        System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase));
                if (employeeProp != null && employeeProp.CanRead)
                {
                    var rawValue = employeeProp.GetValue(existing);
                    currentVal = rawValue?.ToString() ?? "";
                }
                else
                {
                    currentVal = kvp.Value;
                }

                if (!string.IsNullOrEmpty(currentVal))
                {
                    changes.Add(new FieldChangeDto
                    {
                        FieldName = displayNameMap.TryGetValue(kvp.Key, out var dn) ? dn : kvp.Key,
                        OldValue = currentVal,
                        NewValue = null
                    });
                }
            }

            return changes;
        }

        private static string GetDisplayName(Dictionary<string, string> displayNameMap, string propertyName, string fallback)
        {
            return displayNameMap.TryGetValue(propertyName, out var name) ? name : fallback;
        }

        private static Dictionary<string, string> DeserializeAdditionalData(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            try
            {
                var deserialized = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                return deserialized != null
                    ? new Dictionary<string, string>(deserialized, StringComparer.OrdinalIgnoreCase)
                    : new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            }
            catch
            {
                return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            }
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
            table.Columns.Add("AdditionalData", typeof(string));
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
                dr["AdditionalData"] = row.AdditionalFields.Count > 0
                    ? (object?)System.Text.Json.JsonSerializer.Serialize(row.AdditionalFields) ?? DBNull.Value
                    : DBNull.Value;
                dr["ImportedOn"] = now;
                dr["ImportedBy"] = (object?)uploadedBy ?? DBNull.Value;
                table.Rows.Add(dr);
            }

            return table;
        }

        private async Task DistributeAdditionalDataAsync(CancellationToken ct)
        {
            var mappedEntityProps = await GetMappedEntityPropertiesAsync(ct);

            // Step 1: Distribute fields from AdditionalData to Employee entity columns
            var employees = await _dbContext.Employees
                .Where(e => !e.IsDeleted && e.AdditionalData != null && e.AdditionalData != "")
                .ToListAsync(ct);

            var anyChanges = false;

            foreach (var emp in employees)
            {
                Dictionary<string, string>? fields;
                try
                {
                    fields = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(emp.AdditionalData);
                }
                catch
                {
                    continue;
                }

                if (fields == null || fields.Count == 0) continue;

                foreach (var kvp in fields)
                {
                    var prop = EmployeePropertyCache.GetOrAdd(kvp.Key, _ =>
                        EmployeeEntityType.GetProperty(kvp.Key,
                            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase));
                    if (prop == null || !prop.CanWrite) continue;
                    if (!SupportedTypes.Contains(prop.PropertyType)) continue;

                    var converted = ConvertImportValue(kvp.Value, prop.PropertyType);
                    if (converted != null)
                    {
                        prop.SetValue(emp, converted);
                        anyChanges = true;
                    }
                }
            }

            if (anyChanges)
            {
                await _dbContext.SaveChangesAsync(ct);
                _logger.LogInformation("Distributed AdditionalData to actual Employee columns for {Count} employees", employees.Count);
            }

            // Step 2: Clear mapped columns for employees without those fields in AdditionalData (single UPDATE per property)
            foreach (var (name, _) in mappedEntityProps)
            {
                var sql = $"UPDATE Employees SET [{name}] = NULL WHERE IsDeleted = 0 AND [{name}] IS NOT NULL AND (AdditionalData IS NULL OR JSON_VALUE(AdditionalData, N'$.{name}') IS NULL)";
                await _dbContext.Database.ExecuteSqlRawAsync(sql, ct);
            }
        }

        private async Task<List<(string Name, System.Reflection.PropertyInfo Prop)>> GetMappedEntityPropertiesAsync(CancellationToken ct)
        {
            var targetProperties = await _dbContext.ColumnMappings
                .Where(cm => cm.IsActive && cm.EntityType == "employee-import")
                .Select(cm => cm.TargetProperty)
                .Distinct()
                .ToListAsync(ct);

            var result = new List<(string, System.Reflection.PropertyInfo)>();
            foreach (var propName in targetProperties)
            {
                if (FixedColumnProperties.Contains(propName)) continue;

                var prop = EmployeePropertyCache.GetOrAdd(propName, _ =>
                    EmployeeEntityType.GetProperty(propName,
                        System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase));
                if (prop != null && prop.CanWrite && SupportedTypes.Contains(prop.PropertyType))
                    result.Add((propName, prop));
            }
            return result;
        }

        private static object? ConvertImportValue(string value, Type targetType)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            var nonNullableType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            try
            {
                if (nonNullableType == typeof(string))
                    return value;

                if (nonNullableType == typeof(int))
                    return int.TryParse(value, System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out var i) ? i : null;

                if (nonNullableType == typeof(long))
                    return long.TryParse(value, System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out var l) ? l : null;

                if (nonNullableType == typeof(decimal))
                    return decimal.TryParse(value, System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out var d) ? d : null;

                if (nonNullableType == typeof(double))
                    return double.TryParse(value, System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out var dbl) ? dbl : null;

                if (nonNullableType == typeof(float))
                    return float.TryParse(value, System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out var f) ? f : null;

                if (nonNullableType == typeof(bool))
                {
                    if (bool.TryParse(value, out var b)) return b;
                    if (value.Equals("1") || value.Equals("yes", StringComparison.OrdinalIgnoreCase) ||
                        value.Equals("y", StringComparison.OrdinalIgnoreCase)) return true;
                    if (value.Equals("0") || value.Equals("no", StringComparison.OrdinalIgnoreCase) ||
                        value.Equals("n", StringComparison.OrdinalIgnoreCase)) return false;
                    return null;
                }

                if (nonNullableType == typeof(DateTime))
                    return DateTime.TryParse(value, System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.None, out var dt) ? dt : null;

                if (nonNullableType == typeof(Guid))
                    return Guid.TryParse(value, out var g) ? g : null;

                return value;
            }
            catch
            {
                return null;
            }
        }
    }
}