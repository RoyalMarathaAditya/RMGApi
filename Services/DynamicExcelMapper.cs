using System.Collections.Concurrent;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Text.Json;
using HRMS.Api.DTOs.EmployeeDtos;
using HRMS.Api.Models;
using HRMS.Api.Repositories.Interfaces;
using OfficeOpenXml;

namespace HRMS.Api.Services
{
    public class DynamicExcelMapper
    {
        private readonly IColumnMappingRepository _mappingRepository;
        private readonly IColumnValueMappingRepository _valueMappingRepository;
        private readonly ILogger<DynamicExcelMapper> _logger;

        private static readonly ConcurrentDictionary<string, PropertyInfo?> PropertyCache = new();

        public DynamicExcelMapper(
            IColumnMappingRepository mappingRepository,
            IColumnValueMappingRepository valueMappingRepository,
            ILogger<DynamicExcelMapper> logger)
        {
            _mappingRepository = mappingRepository;
            _valueMappingRepository = valueMappingRepository;
            _logger = logger;
        }

        public async Task<(List<EmployeeImportRowDto> Rows, List<UploadColumnInfo> Columns, List<string> Warnings, List<string> Errors)> MapExcelToDtoAsync(
            ExcelWorksheet worksheet, CancellationToken ct)
        {
            var rows = new List<EmployeeImportRowDto>();
            var columns = new List<UploadColumnInfo>();
            var warnings = new List<string>();
            var errors = new List<string>();

            var mappings = (await _mappingRepository.GetByEntityTypeAsync("employee-import", true, ct)).ToList();
            if (mappings.Count == 0)
            {
                errors.Add("No active column mappings found in the database. Please configure column mappings first.");
                return (rows, columns, warnings, errors);
            }

            var valueMappings = (await _valueMappingRepository.GetAllActiveAsync(ct))
                .GroupBy(vm => vm.TargetProperty)
                .ToDictionary(g => g.Key, g => g.ToList());

            var headerMap = BuildHeaderMap(worksheet, mappings, warnings);

            ct.ThrowIfCancellationRequested();

            var requiredMappings = mappings.Where(m => m.IsRequired).ToList();
            var missingRequired = requiredMappings
                .Where(m => !headerMap.ContainsKey(m.SourceColumn))
                .Select(m => m.SourceColumn)
                .ToList();

            if (missingRequired.Any())
            {
                errors.Add($"Missing required column(s) in Excel: {string.Join(", ", missingRequired)}. Please ensure these columns exist with the correct header name in column mappings.");
                return (rows, columns, warnings, errors);
            }

            foreach (var mapping in mappings)
            {
                if (headerMap.ContainsKey(mapping.SourceColumn))
                {
                    columns.Add(new UploadColumnInfo
                    {
                        Field = GetFrontendField(mapping.TargetProperty),
                        Header = mapping.TargetDisplayName
                    });
                }
            }

            var ignoredHeaders = GetIgnoredHeaders(worksheet, mappings);
            foreach (var header in ignoredHeaders)
            {
                warnings.Add($"Ignored unknown column: '{header}'");
            }

            var disabledMappings = mappings.Where(m => !m.IsActive).ToList();
            foreach (var dm in disabledMappings)
            {
                warnings.Add($"Disabled mapping ignored: '{dm.SourceColumn}' -> '{dm.TargetProperty}'");
            }

            var dtoType = typeof(EmployeeImportRowDto);

            for (int row = 2; row <= worksheet.Dimension.Rows; row++)
            {
                ct.ThrowIfCancellationRequested();

                var dto = new EmployeeImportRowDto { RowNumber = row };

                foreach (var mapping in mappings)
                {
                    if (!headerMap.TryGetValue(mapping.SourceColumn, out var colIndex))
                        continue;

                    var rawValue = GetCellRawValue(worksheet, row, colIndex);
                    if (rawValue == null)
                        continue;

                    var prop = GetProperty(dtoType, mapping.TargetProperty);
                    if (prop == null)
                    {
                        warnings.Add($"Target property '{mapping.TargetProperty}' not found on DTO. Skipping.");
                        continue;
                    }

                    var convertedValue = ConvertValue(rawValue, mapping.DataType);
                    if (convertedValue != null)
                    {
                        prop.SetValue(dto, convertedValue);

                        if (valueMappings.TryGetValue(mapping.TargetProperty, out var valueMaps))
                        {
                            var stringVal = convertedValue.ToString();
                            var match = valueMaps.FirstOrDefault(vm =>
                                string.Equals(vm.SourceValue, stringVal, StringComparison.OrdinalIgnoreCase));
                            if (match != null)
                            {
                                prop.SetValue(dto, match.TargetValue);
                                _logger.LogDebug("Value mapped: {Prop} '{Source}' -> '{Target}'",
                                    mapping.TargetProperty, stringVal, match.TargetValue);
                            }
                        }
                    }
                }

                if (IsRowEmpty(dto))
                    continue;

                rows.Add(dto);
            }

            _logger.LogInformation("Mapped {RowCount} rows with {MappingCount} mappings and {ColumnCount} columns",
                rows.Count, mappings.Count, columns.Count);

            return (rows, columns, warnings, errors);
        }

        private static Dictionary<string, int> BuildHeaderMap(
            ExcelWorksheet worksheet, List<ColumnMapping> mappings, List<string> warnings)
        {
            var headerMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var seenHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            for (int col = 1; col <= worksheet.Dimension.Columns; col++)
            {
                var header = worksheet.Cells[1, col].Text?.Trim();
                if (string.IsNullOrEmpty(header)) continue;

                if (!seenHeaders.Add(header))
                {
                    warnings.Add($"Duplicate header found: '{header}'. Using first occurrence.");
                    continue;
                }

                var mapping = mappings.FirstOrDefault(m =>
                    string.Equals(m.SourceColumn, header, StringComparison.OrdinalIgnoreCase));
                if (mapping != null)
                {
                    headerMap[mapping.SourceColumn] = col;
                }
            }

            return headerMap;
        }

        private static List<string> GetIgnoredHeaders(ExcelWorksheet worksheet, List<ColumnMapping> mappings)
        {
            var ignored = new List<string>();
            var mappedColumns = new HashSet<string>(mappings.Select(m => m.SourceColumn), StringComparer.OrdinalIgnoreCase);

            for (int col = 1; col <= worksheet.Dimension.Columns; col++)
            {
                var header = worksheet.Cells[1, col].Text?.Trim();
                if (string.IsNullOrEmpty(header)) continue;
                if (!mappedColumns.Contains(header))
                    ignored.Add(header);
            }

            return ignored;
        }

        private static object? GetCellRawValue(ExcelWorksheet ws, int row, int col)
        {
            var value = ws.Cells[row, col].Value;
            if (value == null) return null;
            if (value is DateTime) return value;
            var text = value.ToString()?.Trim();
            return string.IsNullOrEmpty(text) ? null : text;
        }

        private static object? ConvertValue(object? rawValue, string dataType)
        {
            try
            {
                return dataType.ToLowerInvariant() switch
                {
                    "datetime" => rawValue switch
                    {
                        DateTime dt => dt,
                        string s => ParseDate(s),
                        double d => DateTime.FromOADate(d),
                        _ => null
                    },
                    "int" or "integer" => rawValue switch
                    {
                        int i => i,
                        string s => int.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var i) ? i : null,
                        _ => null
                    },
                    "decimal" or "double" => rawValue switch
                    {
                        decimal d => d,
                        double d => (decimal)d,
                        string s => decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var d) ? d : null,
                        _ => null
                    },
                    "bool" or "boolean" => rawValue switch
                    {
                        bool b => b,
                        string s => bool.TryParse(s, out var b) ? b : null,
                        _ => null
                    },
                    _ => rawValue?.ToString()
                };
            }
            catch
            {
                return rawValue?.ToString();
            }
        }

        private static DateTime? ParseDate(string value)
        {
            if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
                return dt;
            if (DateTime.TryParseExact(value, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                return dt;
            if (DateTime.TryParseExact(value, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                return dt;
            if (DateTime.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                return dt;
            if (DateTime.TryParseExact(value, "dd-MMM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                return dt;
            if (DateTime.TryParseExact(value, "dd MMM yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                return dt;
            if (DateTime.TryParseExact(value, "d MMM yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                return dt;
            return null;
        }

        private static PropertyInfo? GetProperty(Type type, string propertyName)
        {
            return PropertyCache.GetOrAdd($"{type.FullName}.{propertyName}", _ =>
                type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase));
        }

        private static string GetFrontendField(string targetProperty)
        {
            return targetProperty switch
            {
                "EmployeeCode" => "employeeCode",
                "FullName" => "fullName",
                "EmployeeType" => "employmentType",
                "Designation" => "designation",
                "Practice" => "practice",
                "SubPractice" => "subPractice",
                "NVLocation" => "location",
                "ReportingManager" => "reportingManagerName",
                "PracticeHead" => "practiceHeadName",
                "Email" => "email",
                "ActiveStatus" => "employeeStatus",
                "DOJ" => "doj",
                "LWD" => "lwd",
                _ => targetProperty.Length > 0
                    ? char.ToLowerInvariant(targetProperty[0]) + targetProperty[1..]
                    : targetProperty
            };
        }

        private static bool IsRowEmpty(EmployeeImportRowDto dto)
        {
            return string.IsNullOrWhiteSpace(dto.EmployeeCode) &&
                   string.IsNullOrWhiteSpace(dto.FullName) &&
                   string.IsNullOrWhiteSpace(dto.Email) &&
                   string.IsNullOrWhiteSpace(dto.Practice) &&
                   string.IsNullOrWhiteSpace(dto.Designation) &&
                   string.IsNullOrWhiteSpace(dto.EmployeeType);
        }
    }
}
