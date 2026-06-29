using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMS.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddManagerNameColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PracticeHeadName",
                table: "Employees",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReportingManagerName",
                table: "Employees",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_ProcessEmployeeImport");

            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_ProcessEmployeeImport
                    @BatchId UNIQUEIDENTIFIER,
                    @ImportedBy NVARCHAR(200) = NULL
                AS
                BEGIN
                    SET NOCOUNT ON;

                    DECLARE @TotalRows INT = 0;
                    DECLARE @ImportedRows INT = 0;
                    DECLARE @FailedRows INT = 0;
                    DECLARE @DeletedRows INT = 0;

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

                        -- Required field validation
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

                        -- Auto-create missing designations (Role)
                        INSERT INTO DesignationMasters (Id, Name, IsActive, IsDeleted, CreatedOn)
                        SELECT NEWID(), s.Designation, 1, 0, GETUTCDATE()
                        FROM EmployeeImportStaging s
                        WHERE s.BatchId = @BatchId AND s.Designation IS NOT NULL AND s.Designation != ''
                          AND NOT EXISTS (SELECT 1 FROM DesignationMasters d WHERE d.Name = s.Designation)
                        GROUP BY s.Designation;

                        -- Auto-create missing practices (OU 4 - Practice)
                        INSERT INTO Practices (Id, Name, IsActive, IsDeleted, CreatedOn)
                        SELECT NEWID(), s.Practice, 1, 0, GETUTCDATE()
                        FROM EmployeeImportStaging s
                        WHERE s.BatchId = @BatchId AND s.Practice IS NOT NULL AND s.Practice != ''
                          AND NOT EXISTS (SELECT 1 FROM Practices p WHERE p.Name = s.Practice)
                        GROUP BY s.Practice;

                        -- Auto-create missing sub-practices (OU 5 - Sub-practice, linked to Practice)
                        INSERT INTO SubPracticeMasters (Id, Name, PracticeId, IsActive, IsDeleted, CreatedOn)
                        SELECT NEWID(), s.SubPractice, p.Id, 1, 0, GETUTCDATE()
                        FROM EmployeeImportStaging s
                        INNER JOIN Practices p ON p.Name = s.Practice AND p.IsDeleted = 0
                        WHERE s.BatchId = @BatchId AND s.SubPractice IS NOT NULL AND s.SubPractice != ''
                          AND NOT EXISTS (SELECT 1 FROM SubPracticeMasters sp WHERE sp.Name = s.SubPractice AND sp.PracticeId = p.Id)
                        GROUP BY s.SubPractice, p.Id;

                        -- Auto-create missing locations
                        INSERT INTO Locations (Id, Name, Address, IsActive, IsDeleted, CreatedOn)
                        SELECT NEWID(), s.Location, s.Location, 1, 0, GETUTCDATE()
                        FROM EmployeeImportStaging s
                        WHERE s.BatchId = @BatchId AND s.Location IS NOT NULL AND s.Location != ''
                          AND NOT EXISTS (SELECT 1 FROM Locations l WHERE l.Name = s.Location)
                        GROUP BY s.Location;

                        -- Auto-create missing employment types (FTE/ Consultant)
                        INSERT INTO EmploymentTypeMasters (Id, Name, IsActive, IsDeleted, CreatedOn)
                        SELECT NEWID(), s.EmployeeType, 1, 0, GETUTCDATE()
                        FROM EmployeeImportStaging s
                        WHERE s.BatchId = @BatchId AND s.EmployeeType IS NOT NULL AND s.EmployeeType != ''
                          AND NOT EXISTS (SELECT 1 FROM EmploymentTypeMasters e WHERE e.Name = s.EmployeeType)
                        GROUP BY s.EmployeeType;

                        -- Duplicate validation within batch
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

                        -- Auto-create default location for rows without a location
                        IF EXISTS (SELECT 1 FROM EmployeeImportStaging WHERE BatchId = @BatchId AND (Location IS NULL OR Location = ''))
                        BEGIN
                            IF NOT EXISTS (SELECT 1 FROM Locations WHERE Name = 'Not Specified')
                                INSERT INTO Locations (Id, Name, Address, IsActive, IsDeleted, CreatedOn)
                                VALUES (NEWID(), 'Not Specified', 'Not Specified', 1, 0, GETUTCDATE());
                        END

                        BEGIN TRANSACTION;

                        SELECT @DeletedRows = COUNT(1) FROM Employees WHERE IsDeleted = 0;

                        -- Clear all FK references before deleting employees
                        UPDATE Employees SET ReportingManagerId = NULL, PracticeHeadId = NULL;
                        UPDATE Projects SET CSMId = NULL, ProjectManagerId = NULL;
                        UPDATE Practices SET PracticeHeadId = NULL;
                        DELETE FROM EmployeeLeaves;
                        DELETE FROM EmployeeSkills;
                        DELETE FROM PIPs;
                        DELETE FROM ResourceAllocations;
                        DELETE FROM Employees;

                        -- Phase 1: Insert employees with manager names stored directly (no FK resolution)
                        INSERT INTO Employees (
                            EmployeeCode, FullName, Email,
                            DOJ, LWD,
                            DesignationId,
                            EmploymentTypeId, LocationId, PracticeId, SubPracticeId,
                            StatusId,
                            PriorExperience,
                            ReportingManagerName, PracticeHeadName,
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
                            0,
                            s.ReportingManager,
                            s.PracticeHead,
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
                        WHERE s.BatchId = @BatchId;

                        SET @ImportedRows = @@ROWCOUNT;

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
            migrationBuilder.DropColumn(
                name: "PracticeHeadName",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "ReportingManagerName",
                table: "Employees");

            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_ProcessEmployeeImport");

            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_ProcessEmployeeImport
                    @BatchId UNIQUEIDENTIFIER,
                    @ImportedBy NVARCHAR(200) = NULL
                AS
                BEGIN
                    SET NOCOUNT ON;

                    DECLARE @TotalRows INT = 0;
                    DECLARE @ImportedRows INT = 0;
                    DECLARE @FailedRows INT = 0;
                    DECLARE @DeletedRows INT = 0;

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

                        -- Required field validation
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

                        -- Auto-create missing designations (Role)
                        INSERT INTO DesignationMasters (Id, Name, IsActive, IsDeleted, CreatedOn)
                        SELECT NEWID(), s.Designation, 1, 0, GETUTCDATE()
                        FROM EmployeeImportStaging s
                        WHERE s.BatchId = @BatchId AND s.Designation IS NOT NULL AND s.Designation != ''
                          AND NOT EXISTS (SELECT 1 FROM DesignationMasters d WHERE d.Name = s.Designation)
                        GROUP BY s.Designation;

                        -- Auto-create missing practices (OU 4 - Practice)
                        INSERT INTO Practices (Id, Name, IsActive, IsDeleted, CreatedOn)
                        SELECT NEWID(), s.Practice, 1, 0, GETUTCDATE()
                        FROM EmployeeImportStaging s
                        WHERE s.BatchId = @BatchId AND s.Practice IS NOT NULL AND s.Practice != ''
                          AND NOT EXISTS (SELECT 1 FROM Practices p WHERE p.Name = s.Practice)
                        GROUP BY s.Practice;

                        -- Auto-create missing sub-practices (OU 5 - Sub-practice, linked to Practice)
                        INSERT INTO SubPracticeMasters (Id, Name, PracticeId, IsActive, IsDeleted, CreatedOn)
                        SELECT NEWID(), s.SubPractice, p.Id, 1, 0, GETUTCDATE()
                        FROM EmployeeImportStaging s
                        INNER JOIN Practices p ON p.Name = s.Practice AND p.IsDeleted = 0
                        WHERE s.BatchId = @BatchId AND s.SubPractice IS NOT NULL AND s.SubPractice != ''
                          AND NOT EXISTS (SELECT 1 FROM SubPracticeMasters sp WHERE sp.Name = s.SubPractice AND sp.PracticeId = p.Id)
                        GROUP BY s.SubPractice, p.Id;

                        -- Auto-create missing locations
                        INSERT INTO Locations (Id, Name, Address, IsActive, IsDeleted, CreatedOn)
                        SELECT NEWID(), s.Location, s.Location, 1, 0, GETUTCDATE()
                        FROM EmployeeImportStaging s
                        WHERE s.BatchId = @BatchId AND s.Location IS NOT NULL AND s.Location != ''
                          AND NOT EXISTS (SELECT 1 FROM Locations l WHERE l.Name = s.Location)
                        GROUP BY s.Location;

                        -- Auto-create missing employment types (FTE/ Consultant)
                        INSERT INTO EmploymentTypeMasters (Id, Name, IsActive, IsDeleted, CreatedOn)
                        SELECT NEWID(), s.EmployeeType, 1, 0, GETUTCDATE()
                        FROM EmployeeImportStaging s
                        WHERE s.BatchId = @BatchId AND s.EmployeeType IS NOT NULL AND s.EmployeeType != ''
                          AND NOT EXISTS (SELECT 1 FROM EmploymentTypeMasters e WHERE e.Name = s.EmployeeType)
                        GROUP BY s.EmployeeType;

                        -- Duplicate validation within batch
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

                        -- Auto-create default location for rows without a location
                        IF EXISTS (SELECT 1 FROM EmployeeImportStaging WHERE BatchId = @BatchId AND (Location IS NULL OR Location = ''))
                        BEGIN
                            IF NOT EXISTS (SELECT 1 FROM Locations WHERE Name = 'Not Specified')
                                INSERT INTO Locations (Id, Name, Address, IsActive, IsDeleted, CreatedOn)
                                VALUES (NEWID(), 'Not Specified', 'Not Specified', 1, 0, GETUTCDATE());
                        END

                        BEGIN TRANSACTION;

                        SELECT @DeletedRows = COUNT(1) FROM Employees WHERE IsDeleted = 0;

                        -- Preserve employee name-to-ID mapping before deleting
                        CREATE TABLE #EmployeeNameMap (
                            FullName NVARCHAR(200) NOT NULL,
                            EmployeeId INT NOT NULL
                        );

                        INSERT INTO #EmployeeNameMap (FullName, EmployeeId)
                        SELECT FullName, Id
                        FROM Employees
                        WHERE IsDeleted = 0;

                        -- Clear all FK references before deleting employees
                        UPDATE Employees SET ReportingManagerId = NULL, PracticeHeadId = NULL;
                        UPDATE Projects SET CSMId = NULL, ProjectManagerId = NULL;
                        UPDATE Practices SET PracticeHeadId = NULL;
                        DELETE FROM EmployeeLeaves;
                        DELETE FROM EmployeeSkills;
                        DELETE FROM PIPs;
                        DELETE FROM ResourceAllocations;
                        DELETE FROM Employees;

                        -- Phase 1: Insert employees with all FKs except self-referencing (ReportingManagerId, PracticeHeadId)
                        INSERT INTO Employees (
                            EmployeeCode, FullName, Email,
                            DOJ, LWD,
                            DesignationId,
                            EmploymentTypeId, LocationId, PracticeId, SubPracticeId,
                            StatusId,
                            PriorExperience,
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
                            0,
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
                        WHERE s.BatchId = @BatchId;

                        SET @ImportedRows = @@ROWCOUNT;

                        -- Phase 2: Update self-referencing FKs (L1 Manager, Practice Head) by matching full names.
                        UPDATE e
                        SET e.ReportingManagerId = COALESCE(rm_new.Id, rm_old.EmployeeId)
                        FROM Employees e
                        INNER JOIN EmployeeImportStaging s ON s.EmployeeCode = e.EmployeeCode AND s.BatchId = @BatchId
                        LEFT JOIN Employees rm_new ON rm_new.FullName = s.ReportingManager AND rm_new.IsDeleted = 0
                        LEFT JOIN #EmployeeNameMap rm_old ON rm_old.FullName = s.ReportingManager
                        WHERE s.ReportingManager IS NOT NULL AND s.ReportingManager != '';

                        UPDATE e
                        SET e.PracticeHeadId = COALESCE(ph_new.Id, ph_old.EmployeeId)
                        FROM Employees e
                        INNER JOIN EmployeeImportStaging s ON s.EmployeeCode = e.EmployeeCode AND s.BatchId = @BatchId
                        LEFT JOIN Employees ph_new ON ph_new.FullName = s.PracticeHead AND ph_new.IsDeleted = 0
                        LEFT JOIN #EmployeeNameMap ph_old ON ph_old.FullName = s.PracticeHead
                        WHERE s.PracticeHead IS NOT NULL AND s.PracticeHead != '';

                        DROP TABLE #EmployeeNameMap;

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
    }
}
