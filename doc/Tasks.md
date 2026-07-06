# Implementation Tasks

## Dynamic Excel Column Mapping for PeopleStrong Bulk Upload

### Completed Tasks

#### 1. Database Design
- [x] Created `ColumnMappings` table with fields: Id, SourceSystem, SourceColumn, TargetProperty, TargetDisplayName, DataType, IsRequired, IsActive, DisplayOrder, CreatedOn, ModifiedOn
- [x] Created `ColumnValueMappings` table with fields: Id, TargetProperty, SourceValue, TargetValue, IsActive, CreatedOn, ModifiedOn
- [x] Migration `AddColumnMappingTables` created and seeded with PeopleStrong + RMG mappings

#### 2. Entity Models
- [x] Created `Models/ColumnMapping.cs`
- [x] Created `Models/ColumnValueMapping.cs`
- [x] Created EF configurations for both entities

#### 3. Repository Layer
- [x] Created `IColumnMappingRepository` / `ColumnMappingRepository`
- [x] Created `IColumnValueMappingRepository` / `ColumnValueMappingRepository`

#### 4. Service Layer
- [x] Created `IColumnMappingService` / `ColumnMappingService` (CRUD)
- [x] Created `IColumnValueMappingService` / `ColumnValueMappingService` (CRUD)
- [x] Created `DynamicExcelMapper` — core engine using reflection for dynamic Excel-to-DTO mapping with value conversion

#### 5. Admin APIs
- [x] `GET/POST /api/column-mappings` — List/Create column mappings
- [x] `PUT/DELETE /api/column-mappings/{id}` — Update/Delete column mappings
- [x] `GET/POST /api/column-value-mappings` — List/Create value mappings
- [x] `PUT/DELETE /api/column-value-mappings/{id}` — Update/Delete value mappings

#### 6. Modified Bulk Upload Flow
- [x] Added optional `sourceSystem` parameter to `POST /api/employees/bulk-upload`
- [x] `DynamicExcelMapper` loads active mappings, matches Excel headers, uses reflection to set DTO properties
- [x] Value mappings applied automatically (e.g., Permanent → FTE, Active → Active)
- [x] Missing required columns → validation error
- [x] Unknown columns → ignored with warning log
- [x] Fallback to old hardcoded parsing when no source system provided

#### 7. Seed Data
- [x] 13 PeopleStrong column mappings seeded
- [x] 13 RMG template column mappings seeded
- [x] 12 value mappings seeded (EmployeeType + ActiveStatus conversions)

#### 8. Admin UI (Frontend)
- [x] Created `ColumnMappingList` page with full CRUD (table, add/edit dialog, delete confirmation)
- [x] Created `ColumnValueMappingList` page with full CRUD
- [x] Added routes under `/admin/column-mappings` and `/admin/column-value-mappings`
- [x] Sidebar reorganization — moved mapping options under Employee Management as collapsible submenu

#### 9. Dynamic List View Based on Uploaded Columns
- [x] Added `UploadedColumns` field to `EmployeeImportHistory` model
- [x] Added `UploadColumnInfo` DTO (field + header)
- [x] Columns saved to DB during upload, returned in upload response
- [x] `GET /api/employees/last-upload-columns` endpoint fetches last upload columns per user
- [x] Frontend dynamically builds grid columns from API response
- [x] Columns persist across page refresh via backend storage
- [x] Source system selector added to BulkUploadSection
- [x] Removed hardcoded column definitions; grid is fully data-driven

#### 10. Verification
- [x] Backend builds with zero errors
- [x] Frontend TypeScript compilation passes with zero errors
- [x] Frontend production build succeeds
- [x] EF migration generated and validated
