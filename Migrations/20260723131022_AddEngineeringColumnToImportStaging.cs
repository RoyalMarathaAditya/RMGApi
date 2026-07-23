using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMS.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddEngineeringColumnToImportStaging : Migration
    {
        private const string SpName = "sp_ProcessEmployeeImport";

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Engineering",
                table: "EmployeeImportStaging",
                type: "bit",
                nullable: true);

            migrationBuilder.Sql(@"
                MERGE INTO ColumnMappings AS target
                USING (VALUES 
                    ('a1000000-0000-0000-0000-000000000017', 'Engineering', 'Engineering', 'Engineering', 'bool', 17, 'employee-import', 0, 1)
                ) AS source (Id, SourceColumn, TargetProperty, TargetDisplayName, DataType, DisplayOrder, EntityType, IsRequired, IsActive)
                ON target.EntityType = source.EntityType AND target.SourceColumn = source.SourceColumn
                WHEN MATCHED THEN
                    UPDATE SET 
                        TargetProperty = source.TargetProperty,
                        TargetDisplayName = source.TargetDisplayName,
                        DataType = source.DataType,
                        DisplayOrder = source.DisplayOrder,
                        IsRequired = source.IsRequired,
                        IsActive = source.IsActive
                WHEN NOT MATCHED THEN
                    INSERT (Id, SourceColumn, TargetProperty, TargetDisplayName, DataType, DisplayOrder, EntityType, IsRequired, IsActive, CreatedOn)
                    VALUES (CAST(source.Id AS UNIQUEIDENTIFIER), source.SourceColumn, source.TargetProperty, source.TargetDisplayName, source.DataType, source.DisplayOrder, source.EntityType, source.IsRequired, source.IsActive, GETUTCDATE());
            ");

            // Migrate existing Engineering values from AdditionalData JSON to dedicated column
            migrationBuilder.Sql(@"
                UPDATE Employees 
                SET Engineering = 
                    CASE 
                        WHEN JSON_VALUE(AdditionalData, '$.Engineering') = 'true' OR JSON_VALUE(AdditionalData, '$.Engineering') = '1' OR JSON_VALUE(AdditionalData, '$.Engineering') = 'yes' OR JSON_VALUE(AdditionalData, '$.Engineering') = 'y' THEN 1
                        WHEN JSON_VALUE(AdditionalData, '$.Engineering') = 'false' OR JSON_VALUE(AdditionalData, '$.Engineering') = '0' OR JSON_VALUE(AdditionalData, '$.Engineering') = 'no' OR JSON_VALUE(AdditionalData, '$.Engineering') = 'n' THEN 0
                        ELSE NULL
                    END
                WHERE IsDeleted = 0 AND AdditionalData IS NOT NULL AND JSON_VALUE(AdditionalData, '$.Engineering') IS NOT NULL
            ");

            migrationBuilder.Sql($"DROP PROCEDURE IF EXISTS {SpName}");
            migrationBuilder.Sql($@"
                CREATE PROCEDURE {SpName}
                    @BatchId UNIQUEIDENTIFIER,
                    @ImportedBy NVARCHAR(200) = NULL
                AS
                BEGIN
                    SET NOCOUNT ON;

                    DECLARE @TotalRows INT = 0;
                    DECLARE @ImportedRows INT = 0;
                    DECLARE @FailedRows INT = 0;
                    DECLARE @DeletedRows INT = 0;
                    DECLARE @UpdatedRows INT = 0;
                    DECLARE @InsertedRows INT = 0;

                    CREATE TABLE #Errors (
                        RowNum INT IDENTITY(1,1),
                        EmployeeCode NVARCHAR(50),
                        ErrorMessage NVARCHAR(MAX)
                    );

                    BEGIN TRY
                        SELECT @TotalRows = COUNT(1) FROM EmployeeImportStaging WHERE BatchId = @BatchId;

                        IF @TotalRows = 0
                        BEGIN
                            SELECT 0 AS TotalRows, 0 AS ImportedRows, 0 AS FailedRows, 0 AS DeletedRows;
                            RETURN;
                        END

                        INSERT INTO #Errors (EmployeeCode, ErrorMessage)
                        SELECT EmployeeCode, 'Employee Code is required'
                        FROM EmployeeImportStaging
                        WHERE BatchId = @BatchId AND (EmployeeCode IS NULL OR EmployeeCode = '');

                        INSERT INTO #Errors (EmployeeCode, ErrorMessage)
                        SELECT EmployeeCode, 'Email is required'
                        FROM EmployeeImportStaging
                        WHERE BatchId = @BatchId AND (Email IS NULL OR Email = '');

                        INSERT INTO #Errors (EmployeeCode, ErrorMessage)
                        SELECT EmployeeCode, 'Full Name is required'
                        FROM EmployeeImportStaging
                        WHERE BatchId = @BatchId AND (FullName IS NULL OR FullName = '');

                        INSERT INTO DesignationMasters (Id, Name, IsActive, IsDeleted, CreatedOn)
                        SELECT NEWID(), s.Designation, 1, 0, GETUTCDATE()
                        FROM EmployeeImportStaging s
                        WHERE s.BatchId = @BatchId AND s.Designation IS NOT NULL AND s.Designation != ''
                          AND NOT EXISTS (SELECT 1 FROM DesignationMasters d WHERE d.Name = s.Designation)
                        GROUP BY s.Designation;

                        INSERT INTO Practices (Id, Name, IsActive, IsDeleted, CreatedOn)
                        SELECT NEWID(), s.Practice, 1, 0, GETUTCDATE()
                        FROM EmployeeImportStaging s
                        WHERE s.BatchId = @BatchId AND s.Practice IS NOT NULL AND s.Practice != ''
                          AND NOT EXISTS (SELECT 1 FROM Practices p WHERE p.Name = s.Practice)
                        GROUP BY s.Practice;

                        INSERT INTO SubPracticeMasters (Id, Name, PracticeId, IsActive, IsDeleted, CreatedOn)
                        SELECT NEWID(), s.SubPractice, p.Id, 1, 0, GETUTCDATE()
                        FROM EmployeeImportStaging s
                        INNER JOIN Practices p ON p.Name = s.Practice AND p.IsDeleted = 0
                        WHERE s.BatchId = @BatchId AND s.SubPractice IS NOT NULL AND s.SubPractice != ''
                          AND NOT EXISTS (SELECT 1 FROM SubPracticeMasters sp WHERE sp.Name = s.SubPractice AND sp.PracticeId = p.Id)
                        GROUP BY s.SubPractice, p.Id;

                        INSERT INTO Locations (Id, Name, Address, IsActive, IsDeleted, CreatedOn)
                        SELECT NEWID(), s.Location, s.Location, 1, 0, GETUTCDATE()
                        FROM EmployeeImportStaging s
                        WHERE s.BatchId = @BatchId AND s.Location IS NOT NULL AND s.Location != ''
                          AND NOT EXISTS (SELECT 1 FROM Locations l WHERE l.Name = s.Location)
                        GROUP BY s.Location;

                        INSERT INTO EmploymentTypeMasters (Id, Name, IsActive, IsDeleted, CreatedOn)
                        SELECT NEWID(), s.EmployeeType, 1, 0, GETUTCDATE()
                        FROM EmployeeImportStaging s
                        WHERE s.BatchId = @BatchId AND s.EmployeeType IS NOT NULL AND s.EmployeeType != ''
                          AND NOT EXISTS (SELECT 1 FROM EmploymentTypeMasters e WHERE e.Name = s.EmployeeType)
                        GROUP BY s.EmployeeType;

                        INSERT INTO #Errors (EmployeeCode, ErrorMessage)
                        SELECT s.EmployeeCode, 'Duplicate Employee Code ''' + s.EmployeeCode + ''' within upload'
                        FROM EmployeeImportStaging s
                        WHERE s.BatchId = @BatchId AND s.EmployeeCode IS NOT NULL AND s.EmployeeCode != ''
                          AND EXISTS (
                            SELECT 1 FROM EmployeeImportStaging s2
                            WHERE s2.BatchId = @BatchId AND s2.EmployeeCode = s.EmployeeCode AND s2.Id != s.Id
                          );

                        INSERT INTO #Errors (EmployeeCode, ErrorMessage)
                        SELECT s.EmployeeCode, 'Duplicate Email ''' + s.Email + ''' within upload'
                        FROM EmployeeImportStaging s
                        WHERE s.BatchId = @BatchId AND s.Email IS NOT NULL AND s.Email != ''
                          AND EXISTS (
                            SELECT 1 FROM EmployeeImportStaging s2
                            WHERE s2.BatchId = @BatchId AND s2.Email = s.Email AND s2.Id != s.Id
                          );

                        IF EXISTS (SELECT 1 FROM #Errors)
                        BEGIN
                            SET @FailedRows = (SELECT COUNT(DISTINCT EmployeeCode) FROM #Errors WHERE EmployeeCode IS NOT NULL AND EmployeeCode != '');
                            SELECT @TotalRows AS TotalRows, 0 AS ImportedRows, @FailedRows AS FailedRows, 0 AS DeletedRows;
                            DROP TABLE #Errors;
                            RETURN;
                        END

                        IF EXISTS (SELECT 1 FROM EmployeeImportStaging WHERE BatchId = @BatchId AND (Location IS NULL OR Location = ''))
                        BEGIN
                            IF NOT EXISTS (SELECT 1 FROM Locations WHERE Name = 'Not Specified')
                                INSERT INTO Locations (Id, Name, Address, IsActive, IsDeleted, CreatedOn)
                                VALUES (NEWID(), 'Not Specified', 'Not Specified', 1, 0, GETUTCDATE());
                        END

                        BEGIN TRANSACTION;

                        -- Step 1: Update existing employees matched by Email
                        UPDATE e
                        SET 
                            e.EmployeeCode = s.EmployeeCode,
                            e.FullName = s.FullName,
                            e.Email = s.Email,
                            e.DOJ = ISNULL(s.DOJ, GETUTCDATE()),
                            e.LWD = s.LWD,
                            e.DesignationId = dm.Id,
                            e.EmploymentTypeId = etm.Id,
                            e.LocationId = ISNULL(l.Id, (SELECT TOP 1 Id FROM Locations WHERE Name = 'Not Specified' AND IsDeleted = 0)),
                            e.PracticeId = p.Id,
                            e.SubPracticeId = spm.Id,
                            e.StatusId = ISNULL(st.Id, (SELECT TOP 1 Id FROM StatusMasters WHERE Name = 'Active' AND IsDeleted = 0)),
                            e.ReportingManagerName = s.ReportingManager,
                            e.PracticeHeadName = s.PracticeHead,
                            e.PriorExperience = s.PriorExperience,
                            e.RelevantExperience = s.RelevantExperience,
                            e.Engineering = s.Engineering,
                            e.AdditionalData = s.AdditionalData,
                            e.IsDeleted = 0,
                            e.ModifiedOn = GETUTCDATE(),
                            e.ModifiedBy = @ImportedBy
                        FROM Employees e
                        INNER JOIN EmployeeImportStaging s ON s.Email = e.Email AND s.BatchId = @BatchId
                        LEFT JOIN Practices p ON p.Name = s.Practice AND p.IsDeleted = 0
                        LEFT JOIN SubPracticeMasters spm ON spm.Name = s.SubPractice AND spm.PracticeId = p.Id AND spm.IsDeleted = 0
                        LEFT JOIN DesignationMasters dm ON dm.Name = s.Designation AND dm.IsDeleted = 0
                        LEFT JOIN Locations l ON l.Name = ISNULL(NULLIF(s.Location, ''), 'Not Specified') AND l.IsDeleted = 0
                        LEFT JOIN EmploymentTypeMasters etm ON etm.Name = s.EmployeeType AND etm.IsDeleted = 0
                        LEFT JOIN StatusMasters st ON st.Name = ISNULL(s.ActiveStatus, 'Active') AND st.IsDeleted = 0
                        WHERE s.BatchId = @BatchId;

                        SET @UpdatedRows = @@ROWCOUNT;

                        -- Step 2: Insert new employees not already in the Employees table (matched by Email)
                        INSERT INTO Employees (
                            EmployeeCode, FullName, Email,
                            DOJ, LWD,
                            DesignationId,
                            EmploymentTypeId, LocationId, PracticeId, SubPracticeId,
                            StatusId,
                            PriorExperience, RelevantExperience, Engineering,
                            ReportingManagerName, PracticeHeadName,
                            AdditionalData,
                            IsDeleted, CreatedOn, CreatedBy
                        )
                        SELECT
                            s.EmployeeCode,
                            s.FullName,
                            s.Email,
                            ISNULL(s.DOJ, GETUTCDATE()),
                            s.LWD,
                            dm.Id,
                            etm.Id,
                            ISNULL(l.Id, (SELECT TOP 1 Id FROM Locations WHERE Name = 'Not Specified' AND IsDeleted = 0)),
                            p.Id,
                            spm.Id,
                            ISNULL(st.Id, (SELECT TOP 1 Id FROM StatusMasters WHERE Name = 'Active' AND IsDeleted = 0)),
                            s.PriorExperience,
                            s.RelevantExperience,
                            s.Engineering,
                            s.ReportingManager,
                            s.PracticeHead,
                            s.AdditionalData,
                            0,
                            GETUTCDATE(),
                            @ImportedBy
                        FROM EmployeeImportStaging s
                        LEFT JOIN Practices p ON p.Name = s.Practice AND p.IsDeleted = 0
                        LEFT JOIN SubPracticeMasters spm ON spm.Name = s.SubPractice AND spm.PracticeId = p.Id AND spm.IsDeleted = 0
                        LEFT JOIN DesignationMasters dm ON dm.Name = s.Designation AND dm.IsDeleted = 0
                        LEFT JOIN Locations l ON l.Name = ISNULL(NULLIF(s.Location, ''), 'Not Specified') AND l.IsDeleted = 0
                        LEFT JOIN EmploymentTypeMasters etm ON etm.Name = s.EmployeeType AND etm.IsDeleted = 0
                        LEFT JOIN StatusMasters st ON st.Name = ISNULL(s.ActiveStatus, 'Active') AND st.IsDeleted = 0
                        WHERE s.BatchId = @BatchId
                          AND NOT EXISTS (SELECT 1 FROM Employees e WHERE e.Email = s.Email);

                        SET @InsertedRows = @@ROWCOUNT;
                        SET @ImportedRows = @UpdatedRows + @InsertedRows;

                        -- Step 3: Soft delete employees NOT in the uploaded file (matched by Email)
                        UPDATE e
                        SET 
                            e.IsDeleted = 1,
                            e.ModifiedOn = GETUTCDATE(),
                            e.ModifiedBy = @ImportedBy
                        FROM Employees e
                        WHERE e.IsDeleted = 0
                          AND NOT EXISTS (
                            SELECT 1 FROM EmployeeImportStaging s 
                            WHERE s.BatchId = @BatchId AND s.Email = e.Email
                          );

                        SET @DeletedRows = @@ROWCOUNT;

                        DELETE FROM EmployeeImportStaging WHERE BatchId = @BatchId;

                        COMMIT TRANSACTION;

                        SELECT @TotalRows AS TotalRows, @ImportedRows AS ImportedRows, 0 AS FailedRows, @DeletedRows AS DeletedRows;

                    END TRY
                    BEGIN CATCH
                        IF @@TRANCOUNT > 0
                            ROLLBACK TRANSACTION;

                        DELETE FROM EmployeeImportStaging WHERE BatchId = @BatchId;

                        THROW;
                    END CATCH;

                    DROP TABLE #Errors;
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"DROP PROCEDURE IF EXISTS {SpName}");

            // Restore SP without Engineering (same as previous migration's SP)
            migrationBuilder.Sql($@"
                CREATE PROCEDURE {SpName}
                    @BatchId UNIQUEIDENTIFIER,
                    @ImportedBy NVARCHAR(200) = NULL
                AS
                BEGIN
                    SET NOCOUNT ON;

                    DECLARE @TotalRows INT = 0;
                    DECLARE @ImportedRows INT = 0;
                    DECLARE @FailedRows INT = 0;
                    DECLARE @DeletedRows INT = 0;
                    DECLARE @UpdatedRows INT = 0;
                    DECLARE @InsertedRows INT = 0;

                    CREATE TABLE #Errors (
                        RowNum INT IDENTITY(1,1),
                        EmployeeCode NVARCHAR(50),
                        ErrorMessage NVARCHAR(MAX)
                    );

                    BEGIN TRY
                        SELECT @TotalRows = COUNT(1) FROM EmployeeImportStaging WHERE BatchId = @BatchId;

                        IF @TotalRows = 0
                        BEGIN
                            SELECT 0 AS TotalRows, 0 AS ImportedRows, 0 AS FailedRows, 0 AS DeletedRows;
                            RETURN;
                        END

                        INSERT INTO #Errors (EmployeeCode, ErrorMessage)
                        SELECT EmployeeCode, 'Employee Code is required'
                        FROM EmployeeImportStaging
                        WHERE BatchId = @BatchId AND (EmployeeCode IS NULL OR EmployeeCode = '');

                        INSERT INTO #Errors (EmployeeCode, ErrorMessage)
                        SELECT EmployeeCode, 'Email is required'
                        FROM EmployeeImportStaging
                        WHERE BatchId = @BatchId AND (Email IS NULL OR Email = '');

                        INSERT INTO #Errors (EmployeeCode, ErrorMessage)
                        SELECT EmployeeCode, 'Full Name is required'
                        FROM EmployeeImportStaging
                        WHERE BatchId = @BatchId AND (FullName IS NULL OR FullName = '');

                        INSERT INTO DesignationMasters (Id, Name, IsActive, IsDeleted, CreatedOn)
                        SELECT NEWID(), s.Designation, 1, 0, GETUTCDATE()
                        FROM EmployeeImportStaging s
                        WHERE s.BatchId = @BatchId AND s.Designation IS NOT NULL AND s.Designation != ''
                          AND NOT EXISTS (SELECT 1 FROM DesignationMasters d WHERE d.Name = s.Designation)
                        GROUP BY s.Designation;

                        INSERT INTO Practices (Id, Name, IsActive, IsDeleted, CreatedOn)
                        SELECT NEWID(), s.Practice, 1, 0, GETUTCDATE()
                        FROM EmployeeImportStaging s
                        WHERE s.BatchId = @BatchId AND s.Practice IS NOT NULL AND s.Practice != ''
                          AND NOT EXISTS (SELECT 1 FROM Practices p WHERE p.Name = s.Practice)
                        GROUP BY s.Practice;

                        INSERT INTO SubPracticeMasters (Id, Name, PracticeId, IsActive, IsDeleted, CreatedOn)
                        SELECT NEWID(), s.SubPractice, p.Id, 1, 0, GETUTCDATE()
                        FROM EmployeeImportStaging s
                        INNER JOIN Practices p ON p.Name = s.Practice AND p.IsDeleted = 0
                        WHERE s.BatchId = @BatchId AND s.SubPractice IS NOT NULL AND s.SubPractice != ''
                          AND NOT EXISTS (SELECT 1 FROM SubPracticeMasters sp WHERE sp.Name = s.SubPractice AND sp.PracticeId = p.Id)
                        GROUP BY s.SubPractice, p.Id;

                        INSERT INTO Locations (Id, Name, Address, IsActive, IsDeleted, CreatedOn)
                        SELECT NEWID(), s.Location, s.Location, 1, 0, GETUTCDATE()
                        FROM EmployeeImportStaging s
                        WHERE s.BatchId = @BatchId AND s.Location IS NOT NULL AND s.Location != ''
                          AND NOT EXISTS (SELECT 1 FROM Locations l WHERE l.Name = s.Location)
                        GROUP BY s.Location;

                        INSERT INTO EmploymentTypeMasters (Id, Name, IsActive, IsDeleted, CreatedOn)
                        SELECT NEWID(), s.EmployeeType, 1, 0, GETUTCDATE()
                        FROM EmployeeImportStaging s
                        WHERE s.BatchId = @BatchId AND s.EmployeeType IS NOT NULL AND s.EmployeeType != ''
                          AND NOT EXISTS (SELECT 1 FROM EmploymentTypeMasters e WHERE e.Name = s.EmployeeType)
                        GROUP BY s.EmployeeType;

                        INSERT INTO #Errors (EmployeeCode, ErrorMessage)
                        SELECT s.EmployeeCode, 'Duplicate Employee Code ''' + s.EmployeeCode + ''' within upload'
                        FROM EmployeeImportStaging s
                        WHERE s.BatchId = @BatchId AND s.EmployeeCode IS NOT NULL AND s.EmployeeCode != ''
                          AND EXISTS (
                            SELECT 1 FROM EmployeeImportStaging s2
                            WHERE s2.BatchId = @BatchId AND s2.EmployeeCode = s.EmployeeCode AND s2.Id != s.Id
                          );

                        INSERT INTO #Errors (EmployeeCode, ErrorMessage)
                        SELECT s.EmployeeCode, 'Duplicate Email ''' + s.Email + ''' within upload'
                        FROM EmployeeImportStaging s
                        WHERE s.BatchId = @BatchId AND s.Email IS NOT NULL AND s.Email != ''
                          AND EXISTS (
                            SELECT 1 FROM EmployeeImportStaging s2
                            WHERE s2.BatchId = @BatchId AND s2.Email = s.Email AND s2.Id != s.Id
                          );

                        IF EXISTS (SELECT 1 FROM #Errors)
                        BEGIN
                            SET @FailedRows = (SELECT COUNT(DISTINCT EmployeeCode) FROM #Errors WHERE EmployeeCode IS NOT NULL AND EmployeeCode != '');
                            SELECT @TotalRows AS TotalRows, 0 AS ImportedRows, @FailedRows AS FailedRows, 0 AS DeletedRows;
                            DROP TABLE #Errors;
                            RETURN;
                        END

                        IF EXISTS (SELECT 1 FROM EmployeeImportStaging WHERE BatchId = @BatchId AND (Location IS NULL OR Location = ''))
                        BEGIN
                            IF NOT EXISTS (SELECT 1 FROM Locations WHERE Name = 'Not Specified')
                                INSERT INTO Locations (Id, Name, Address, IsActive, IsDeleted, CreatedOn)
                                VALUES (NEWID(), 'Not Specified', 'Not Specified', 1, 0, GETUTCDATE());
                        END

                        BEGIN TRANSACTION;

                        -- Step 1: Update existing employees matched by Email
                        UPDATE e
                        SET 
                            e.EmployeeCode = s.EmployeeCode,
                            e.FullName = s.FullName,
                            e.Email = s.Email,
                            e.DOJ = ISNULL(s.DOJ, GETUTCDATE()),
                            e.LWD = s.LWD,
                            e.DesignationId = dm.Id,
                            e.EmploymentTypeId = etm.Id,
                            e.LocationId = ISNULL(l.Id, (SELECT TOP 1 Id FROM Locations WHERE Name = 'Not Specified' AND IsDeleted = 0)),
                            e.PracticeId = p.Id,
                            e.SubPracticeId = spm.Id,
                            e.StatusId = ISNULL(st.Id, (SELECT TOP 1 Id FROM StatusMasters WHERE Name = 'Active' AND IsDeleted = 0)),
                            e.ReportingManagerName = s.ReportingManager,
                            e.PracticeHeadName = s.PracticeHead,
                            e.PriorExperience = s.PriorExperience,
                            e.RelevantExperience = s.RelevantExperience,
                            e.AdditionalData = s.AdditionalData,
                            e.IsDeleted = 0,
                            e.ModifiedOn = GETUTCDATE(),
                            e.ModifiedBy = @ImportedBy
                        FROM Employees e
                        INNER JOIN EmployeeImportStaging s ON s.Email = e.Email AND s.BatchId = @BatchId
                        LEFT JOIN Practices p ON p.Name = s.Practice AND p.IsDeleted = 0
                        LEFT JOIN SubPracticeMasters spm ON spm.Name = s.SubPractice AND spm.PracticeId = p.Id AND spm.IsDeleted = 0
                        LEFT JOIN DesignationMasters dm ON dm.Name = s.Designation AND dm.IsDeleted = 0
                        LEFT JOIN Locations l ON l.Name = ISNULL(NULLIF(s.Location, ''), 'Not Specified') AND l.IsDeleted = 0
                        LEFT JOIN EmploymentTypeMasters etm ON etm.Name = s.EmployeeType AND etm.IsDeleted = 0
                        LEFT JOIN StatusMasters st ON st.Name = ISNULL(s.ActiveStatus, 'Active') AND st.IsDeleted = 0
                        WHERE s.BatchId = @BatchId;

                        SET @UpdatedRows = @@ROWCOUNT;

                        -- Step 2: Insert new employees not already in the Employees table (matched by Email)
                        INSERT INTO Employees (
                            EmployeeCode, FullName, Email,
                            DOJ, LWD,
                            DesignationId,
                            EmploymentTypeId, LocationId, PracticeId, SubPracticeId,
                            StatusId,
                            PriorExperience, RelevantExperience,
                            ReportingManagerName, PracticeHeadName,
                            AdditionalData,
                            IsDeleted, CreatedOn, CreatedBy
                        )
                        SELECT
                            s.EmployeeCode,
                            s.FullName,
                            s.Email,
                            ISNULL(s.DOJ, GETUTCDATE()),
                            s.LWD,
                            dm.Id,
                            etm.Id,
                            ISNULL(l.Id, (SELECT TOP 1 Id FROM Locations WHERE Name = 'Not Specified' AND IsDeleted = 0)),
                            p.Id,
                            spm.Id,
                            ISNULL(st.Id, (SELECT TOP 1 Id FROM StatusMasters WHERE Name = 'Active' AND IsDeleted = 0)),
                            s.PriorExperience,
                            s.RelevantExperience,
                            s.ReportingManager,
                            s.PracticeHead,
                            s.AdditionalData,
                            0,
                            GETUTCDATE(),
                            @ImportedBy
                        FROM EmployeeImportStaging s
                        LEFT JOIN Practices p ON p.Name = s.Practice AND p.IsDeleted = 0
                        LEFT JOIN SubPracticeMasters spm ON spm.Name = s.SubPractice AND spm.PracticeId = p.Id AND spm.IsDeleted = 0
                        LEFT JOIN DesignationMasters dm ON dm.Name = s.Designation AND dm.IsDeleted = 0
                        LEFT JOIN Locations l ON l.Name = ISNULL(NULLIF(s.Location, ''), 'Not Specified') AND l.IsDeleted = 0
                        LEFT JOIN EmploymentTypeMasters etm ON etm.Name = s.EmployeeType AND etm.IsDeleted = 0
                        LEFT JOIN StatusMasters st ON st.Name = ISNULL(s.ActiveStatus, 'Active') AND st.IsDeleted = 0
                        WHERE s.BatchId = @BatchId
                          AND NOT EXISTS (SELECT 1 FROM Employees e WHERE e.Email = s.Email);

                        SET @InsertedRows = @@ROWCOUNT;
                        SET @ImportedRows = @UpdatedRows + @InsertedRows;

                        -- Step 3: Soft delete employees NOT in the uploaded file (matched by Email)
                        UPDATE e
                        SET 
                            e.IsDeleted = 1,
                            e.ModifiedOn = GETUTCDATE(),
                            e.ModifiedBy = @ImportedBy
                        FROM Employees e
                        WHERE e.IsDeleted = 0
                          AND NOT EXISTS (
                            SELECT 1 FROM EmployeeImportStaging s 
                            WHERE s.BatchId = @BatchId AND s.Email = e.Email
                          );

                        SET @DeletedRows = @@ROWCOUNT;

                        DELETE FROM EmployeeImportStaging WHERE BatchId = @BatchId;

                        COMMIT TRANSACTION;

                        SELECT @TotalRows AS TotalRows, @ImportedRows AS ImportedRows, 0 AS FailedRows, @DeletedRows AS DeletedRows;

                    END TRY
                    BEGIN CATCH
                        IF @@TRANCOUNT > 0
                            ROLLBACK TRANSACTION;

                        DELETE FROM EmployeeImportStaging WHERE BatchId = @BatchId;

                        THROW;
                    END CATCH;

                    DROP TABLE #Errors;
                END
            ");

            migrationBuilder.DropColumn(
                name: "Engineering",
                table: "EmployeeImportStaging");
        }
    }
}
