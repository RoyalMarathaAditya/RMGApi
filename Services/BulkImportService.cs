using System.Globalization;
using HRMS.Api.Data;
using HRMS.Api.DTOs.EmployeeDtos;
using HRMS.Api.Models;
using HRMS.Api.Validators;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace HRMS.Api.Services
{
    public class BulkImportService : IBulkImportService
    {
        private readonly AppDbContext _dbContext;
        private static readonly HashSet<string> EmailSet = new(StringComparer.OrdinalIgnoreCase);

        static BulkImportService()
        {
            ExcelPackage.License.SetNonCommercialOrganization("RMG HRMS");
        }

        public BulkImportService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
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
                return result;
            }

            result.SuccessRows = result.TotalRows;

            var autoCodeCounter = await GetNextEmployeeCodeNumberAsync(cancellationToken);

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var masterCache = new MasterCache(_dbContext);
                await masterCache.InitializeAsync(cancellationToken);

                foreach (var row in rows)
                {
                    var empTypeId = await masterCache.GetOrCreateEmploymentTypeAsync(row.EmployeeType, cancellationToken);
                    var workModelId = await masterCache.GetOrCreateWorkModelAsync(row.WorkModel, cancellationToken);
                    var practiceId = await masterCache.GetOrCreatePracticeAsync(row.Practice, cancellationToken);
                    var locationId = await masterCache.GetOrCreateLocationAsync(row.NVLocation, cancellationToken);
                    var designationId = await masterCache.GetOrCreateDesignationAsync(row.Designation, cancellationToken);
                    var onboardingId = await masterCache.GetOrCreateOnboardingTypeAsync(row.Onboarding, cancellationToken);
                    var clientId = await masterCache.GetOrCreateClientAsync(row.Client, cancellationToken);
                    var statusId = await masterCache.GetDefaultStatusIdAsync(cancellationToken);
                    var deptTypeId = await masterCache.GetDefaultDepartmentTypeIdAsync(cancellationToken);

                    var reportingManagerId = await FindOrWarnEmployeeAsync(row.ReportingManager, result.Errors, row, "Reporting manager", cancellationToken);
                    var practiceHeadId = await FindOrWarnEmployeeAsync(row.PracticeHead, result.Errors, row, "Practice head", cancellationToken);

                    string employeeCode;
                    if (!string.IsNullOrWhiteSpace(row.EmployeeCode))
                    {
                        employeeCode = row.EmployeeCode;
                    }
                    else
                    {
                        employeeCode = $"EMP{autoCodeCounter:D4}";
                        autoCodeCounter++;
                    }

                    var employee = new Employee
                    {
                        FirstName = row.FirstName,
                        LastName = row.LastName,
                        FullName = $"{row.FirstName} {row.LastName}".Trim(),
                        EmployeeCode = employeeCode,
                        Email = row.Email,
                        DOJ = row.DOJ ?? DateTime.UtcNow,
                        EmploymentTypeId = empTypeId ?? Guid.Empty,
                        WorkModelId = workModelId ?? Guid.Empty,
                        PracticeId = practiceId ?? Guid.Empty,
                        LocationId = locationId ?? Guid.Empty,
                        DesignationId = designationId,
                        ClientId = clientId,
                        OnboardingTypeId = onboardingId,
                        StatusId = statusId,
                        DepartmentTypeId = deptTypeId,
                        ReportingManagerId = reportingManagerId,
                        PracticeHeadId = practiceHeadId,
                        MobileNumber = row.PhoneNumber,
                        PriorExperience = row.Experience ?? 0,
                        RelevantExperience = row.Experience,
                        ExperienceYears = row.Experience,
                        IsDeleted = false,
                        CreatedOn = DateTime.UtcNow,
                    };

                    _dbContext.Employees.Add(employee);
                }

                await _dbContext.SaveChangesAsync(cancellationToken);

                var audit = new ImportAudit
                {
                    FileName = file.FileName,
                    TotalRows = result.TotalRows,
                    SuccessRows = result.SuccessRows,
                    FailedRows = result.FailedRows,
                    UploadedBy = uploadedBy,
                    UploadedOn = DateTime.UtcNow
                };
                _dbContext.ImportAudits.Add(audit);
                await _dbContext.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);
                result.Success = true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }

            return result;
        }

        public byte[] GenerateTemplate()
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Template");

            var headers = new[]
            {
                "Employee Code", "First Name", "Second Name", "Employee Type", "Designation",
                "Practice Head", "Reporting Manager", "Practice", "Client",
                "DOJ", "NV Location", "Work Model", "Onboarding",
                "Phone Number", "E-mail ID", "Experience"
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
                    EmployeeCode = GetString(worksheet, row, headerMap, "Employee Code") ?? GetString(worksheet, row, headerMap, "Emp Code"),
                    FirstName = GetString(worksheet, row, headerMap, "First Name"),
                    LastName = GetString(worksheet, row, headerMap, "Second Name"),
                    EmployeeType = GetString(worksheet, row, headerMap, "Employee Type"),
                    Designation = GetString(worksheet, row, headerMap, "Designation"),
                    PracticeHead = GetString(worksheet, row, headerMap, "Practice Head"),
                    ReportingManager = GetString(worksheet, row, headerMap, "Reporting Manager"),
                    Practice = GetString(worksheet, row, headerMap, "Practice"),
                    Client = GetString(worksheet, row, headerMap, "Client"),
                    NVLocation = GetString(worksheet, row, headerMap, "NV Location"),
                    WorkModel = GetString(worksheet, row, headerMap, "Work Model"),
                    Onboarding = GetString(worksheet, row, headerMap, "Onboarding"),
                    PhoneNumber = GetString(worksheet, row, headerMap, "Phone Number"),
                    Email = GetString(worksheet, row, headerMap, "E-mail ID") ?? GetString(worksheet, row, headerMap, "Email"),
                    DOJ = ParseDate(GetString(worksheet, row, headerMap, "DOJ")),
                    Experience = ParseDecimal(GetString(worksheet, row, headerMap, "Experience")),
                };
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

        private static DateTime? ParseDate(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                return date;
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
            var validator = new EmployeeImportRowValidator();
            var emailSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var existingEmails = await _dbContext.Employees
                .IgnoreQueryFilters()
                .Select(e => e.Email)
                .ToListAsync(cancellationToken);
            var existingEmailSet = new HashSet<string>(existingEmails, StringComparer.OrdinalIgnoreCase);

            var employeeCodeSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var existingCodes = await _dbContext.Employees
                .IgnoreQueryFilters()
                .Select(e => e.EmployeeCode)
                .ToListAsync(cancellationToken);
            var existingCodeSet = new HashSet<string>(existingCodes, StringComparer.OrdinalIgnoreCase);

            foreach (var row in rows)
            {
                var rowErrors = new List<string>();

                var validationResult = await validator.ValidateAsync(row, cancellationToken);
                rowErrors.AddRange(validationResult.Errors.Select(e => e.ErrorMessage));

                if (!string.IsNullOrWhiteSpace(row.EmployeeCode))
                {
                    if (!employeeCodeSet.Add(row.EmployeeCode))
                        rowErrors.Add("Duplicate employee code found within the uploaded file.");

                    if (existingCodeSet.Contains(row.EmployeeCode))
                        rowErrors.Add("Employee code already exists in the system.");
                }

                if (!string.IsNullOrEmpty(row.Email))
                {
                    if (!emailSet.Add(row.Email))
                        rowErrors.Add("Duplicate email found within the uploaded file.");

                    if (existingEmailSet.Contains(row.Email))
                        rowErrors.Add("Email already exists in the system.");
                }

                if (rowErrors.Any())
                {
                    errors.Add(new EmployeeImportErrorDto
                    {
                        RowNumber = row.RowNumber,
                        EmployeeName = $"{row.FirstName} {row.LastName}".Trim(),
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

        private async Task<int?> FindOrWarnEmployeeAsync(string? name, List<EmployeeImportErrorDto> warnings, EmployeeImportRowDto row, string label, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;

            var employee = await _dbContext.Employees
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(e => e.FullName == name || e.Email == name, cancellationToken);

            if (employee != null) return employee.Id;

            warnings.Add(new EmployeeImportErrorDto
            {
                RowNumber = row.RowNumber,
                EmployeeName = $"{row.FirstName} {row.LastName}".Trim(),
                Email = row.Email,
                ErrorMessage = $"{label} '{name}' not found. Set to null."
            });

            return null;
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

        private class MasterCache
        {
            private readonly AppDbContext _db;
            private Dictionary<string, Guid>? _employmentTypes;
            private Dictionary<string, Guid>? _workModels;
            private Dictionary<string, Guid>? _practices;
            private Dictionary<string, Guid>? _locations;
            private Dictionary<string, Guid>? _designations;
            private Dictionary<string, Guid>? _onboardingTypes;
            private Dictionary<string, int>? _clients;
            private Guid? _defaultStatusId;
            private Guid? _defaultDepartmentTypeId;

            public MasterCache(AppDbContext db)
            {
                _db = db;
            }

            public async Task InitializeAsync(CancellationToken ct)
            {
                _employmentTypes = await _db.EmploymentTypeMasters
                    .IgnoreQueryFilters()
                    .ToDictionaryAsync(e => e.Name, e => e.Id, StringComparer.OrdinalIgnoreCase, ct);

                _workModels = await _db.WorkModelMasters
                    .IgnoreQueryFilters()
                    .ToDictionaryAsync(w => w.Name, w => w.Id, StringComparer.OrdinalIgnoreCase, ct);

                _practices = await _db.Practices
                    .IgnoreQueryFilters()
                    .ToDictionaryAsync(p => p.Name, p => p.Id, StringComparer.OrdinalIgnoreCase, ct);

                _locations = await _db.Locations
                    .IgnoreQueryFilters()
                    .ToDictionaryAsync(l => l.Name, l => l.Id, StringComparer.OrdinalIgnoreCase, ct);

                _designations = await _db.DesignationMasters
                    .IgnoreQueryFilters()
                    .ToDictionaryAsync(d => d.Name, d => d.Id, StringComparer.OrdinalIgnoreCase, ct);

                _onboardingTypes = await _db.OnboardingTypeMasters
                    .IgnoreQueryFilters()
                    .ToDictionaryAsync(o => o.Name, o => o.Id, StringComparer.OrdinalIgnoreCase, ct);

                _clients = await _db.Clients
                    .IgnoreQueryFilters()
                    .ToDictionaryAsync(c => c.Name, c => c.Id, StringComparer.OrdinalIgnoreCase, ct);

                _defaultStatusId = await _db.StatusMasters
                    .IgnoreQueryFilters()
                    .Where(s => s.IsActive)
                    .Select(s => (Guid?)s.Id)
                    .FirstOrDefaultAsync(ct);

                _defaultDepartmentTypeId = await _db.DepartmentTypeMasters
                    .IgnoreQueryFilters()
                    .Where(d => d.IsActive)
                    .Select(d => (Guid?)d.Id)
                    .FirstOrDefaultAsync(ct);
            }

            public async Task<Guid> GetDefaultDepartmentTypeIdAsync(CancellationToken ct)
            {
                if (_defaultDepartmentTypeId.HasValue) return _defaultDepartmentTypeId.Value;
                var deptType = await _db.DepartmentTypeMasters
                    .IgnoreQueryFilters()
                    .FirstAsync(ct);
                _defaultDepartmentTypeId = deptType.Id;
                return deptType.Id;
            }

            public async Task<Guid?> GetOrCreateEmploymentTypeAsync(string? name, CancellationToken ct)
            {
                if (string.IsNullOrWhiteSpace(name)) return null;
                return await GetOrCreateGuidAsync(name, _employmentTypes!, _db.EmploymentTypeMasters, ct);
            }

            public async Task<Guid?> GetOrCreateWorkModelAsync(string? name, CancellationToken ct)
            {
                if (string.IsNullOrWhiteSpace(name)) return null;
                return await GetOrCreateGuidAsync(name, _workModels!, _db.WorkModelMasters, ct);
            }

            public async Task<Guid?> GetOrCreatePracticeAsync(string? name, CancellationToken ct)
            {
                if (string.IsNullOrWhiteSpace(name)) return null;
                return await GetOrCreateGuidAsync(name, _practices!, _db.Practices, ct, isPractice: true);
            }

            public async Task<Guid?> GetOrCreateLocationAsync(string? name, CancellationToken ct)
            {
                if (string.IsNullOrWhiteSpace(name)) return null;
                return await GetOrCreateGuidAsync(name, _locations!, _db.Locations, ct, isLocation: true);
            }

            public async Task<Guid?> GetOrCreateDesignationAsync(string? name, CancellationToken ct)
            {
                if (string.IsNullOrWhiteSpace(name)) return null;
                return await GetOrCreateGuidAsync(name, _designations!, _db.DesignationMasters, ct, isDesignation: true);
            }

            public async Task<Guid?> GetOrCreateOnboardingTypeAsync(string? name, CancellationToken ct)
            {
                if (string.IsNullOrWhiteSpace(name)) return null;
                return await GetOrCreateGuidAsync(name, _onboardingTypes!, _db.OnboardingTypeMasters, ct, isOnboarding: true);
            }

            public async Task<int?> GetOrCreateClientAsync(string? name, CancellationToken ct)
            {
                if (string.IsNullOrWhiteSpace(name)) return null;

                if (_clients!.TryGetValue(name, out var id))
                    return id;

                var existing = await _db.Clients
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(c => c.Name == name, ct);
                if (existing != null)
                {
                    _clients[name] = existing.Id;
                    return existing.Id;
                }

                var defaultStatusId = await _db.StatusMasters
                    .IgnoreQueryFilters()
                    .Where(s => s.IsActive)
                    .Select(s => s.Id)
                    .FirstAsync(ct);

                var client = new Client
                {
                    Name = name,
                    StatusId = defaultStatusId,
                    CreatedOn = DateTime.UtcNow
                };
                _db.Clients.Add(client);
                await _db.SaveChangesAsync(ct);
                _clients[name] = client.Id;
                return client.Id;
            }

            public async Task<Guid> GetDefaultStatusIdAsync(CancellationToken ct)
            {
                if (_defaultStatusId.HasValue) return _defaultStatusId.Value;
                var status = await _db.StatusMasters
                    .IgnoreQueryFilters()
                    .FirstAsync(ct);
                _defaultStatusId = status.Id;
                return status.Id;
            }

            private async Task<Guid?> GetOrCreateGuidAsync<T>(
                string name,
                Dictionary<string, Guid> cache,
                DbSet<T> dbSet,
                CancellationToken ct,
                bool isPractice = false,
                bool isLocation = false,
                bool isDesignation = false,
                bool isOnboarding = false) where T : class
            {
                if (cache.TryGetValue(name, out var id))
                    return id;

                object? existing = null;
                if (isPractice)
                    existing = await dbSet.IgnoreQueryFilters().OfType<Practice>().FirstOrDefaultAsync(p => p.Name == name, ct);
                else if (isLocation)
                    existing = await dbSet.IgnoreQueryFilters().OfType<Location>().FirstOrDefaultAsync(l => l.Name == name, ct);
                else if (isDesignation)
                    existing = await dbSet.IgnoreQueryFilters().OfType<DesignationMaster>().FirstOrDefaultAsync(d => d.Name == name, ct);
                else if (isOnboarding)
                    existing = await dbSet.IgnoreQueryFilters().OfType<OnboardingTypeMaster>().FirstOrDefaultAsync(o => o.Name == name, ct);
                else
                    existing = await dbSet.IgnoreQueryFilters().FirstOrDefaultAsync(
                        e => EF.Property<string>(e, "Name") == name, ct);

                if (existing is BaseMasterEntity masterEntity && masterEntity.Id != Guid.Empty)
                {
                    cache[name] = masterEntity.Id;
                    return masterEntity.Id;
                }
                if (existing is Practice p && p.Id != Guid.Empty) { cache[name] = p.Id; return p.Id; }
                if (existing is Location loc && loc.Id != Guid.Empty) { cache[name] = loc.Id; return loc.Id; }

                var newId = Guid.NewGuid();
                var newEntity = Activator.CreateInstance<T>()!;

                var idProp = typeof(T).GetProperty("Id");
                idProp?.SetValue(newEntity, newId);

                var nameProp = typeof(T).GetProperty("Name");
                nameProp?.SetValue(newEntity, name);

                var isDeletedProp = typeof(T).GetProperty("IsDeleted");
                isDeletedProp?.SetValue(newEntity, false);

                var isActiveProp = typeof(T).GetProperty("IsActive");
                isActiveProp?.SetValue(newEntity, true);

                var createdOnProp = typeof(T).GetProperty("CreatedOn");
                createdOnProp?.SetValue(newEntity, DateTime.UtcNow);

                dbSet.Add(newEntity);
                await _db.SaveChangesAsync(ct);
                cache[name] = newId;
                return newId;
            }
        }
    }
}
