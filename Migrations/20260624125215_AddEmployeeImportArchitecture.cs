using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMS.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeImportArchitecture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE TABLE EmployeeImportStaging (
                    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
                    BatchId UNIQUEIDENTIFIER NOT NULL,
                    EmployeeCode NVARCHAR(50) NULL,
                    FirstName NVARCHAR(100) NULL,
                    LastName NVARCHAR(100) NULL,
                    Email NVARCHAR(200) NULL,
                    EmployeeType NVARCHAR(100) NULL,
                    Designation NVARCHAR(100) NULL,
                    Practice NVARCHAR(100) NULL,
                    ReportingManager NVARCHAR(200) NULL,
                    Location NVARCHAR(100) NULL,
                    WorkModel NVARCHAR(100) NULL,
                    Experience DECIMAL(10,2) NULL,
                    Skills NVARCHAR(MAX) NULL,
                    DOJ DATETIME NULL,
                    PhoneNumber NVARCHAR(20) NULL,
                    Client NVARCHAR(200) NULL,
                    Onboarding NVARCHAR(100) NULL,
                    PracticeHead NVARCHAR(200) NULL,
                    SubPractice NVARCHAR(100) NULL,
                    ImportedOn DATETIME NOT NULL DEFAULT GETUTCDATE(),
                    ImportedBy NVARCHAR(200) NULL
                );
            ");

            migrationBuilder.Sql(@"
                CREATE TABLE EmployeeImportHistory (
                    BatchId UNIQUEIDENTIFIER PRIMARY KEY,
                    FileName NVARCHAR(255) NOT NULL,
                    ImportedBy NVARCHAR(200) NULL,
                    ImportedOn DATETIME NOT NULL DEFAULT GETUTCDATE(),
                    TotalRows INT NOT NULL DEFAULT 0,
                    ImportedRows INT NOT NULL DEFAULT 0,
                    FailedRows INT NOT NULL DEFAULT 0,
                    DeletedRows INT NOT NULL DEFAULT 0,
                    Status NVARCHAR(50) NOT NULL DEFAULT 'Completed'
                );
            ");

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
                    DECLARE @DefaultDeptTypeId UNIQUEIDENTIFIER;
                    DECLARE @DefaultStatusId UNIQUEIDENTIFIER;

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

                        SELECT TOP 1 @DefaultDeptTypeId = Id FROM DepartmentTypeMasters WHERE IsDeleted = 0 AND IsActive = 1;
                        SELECT TOP 1 @DefaultStatusId = Id FROM StatusMasters WHERE IsDeleted = 0 AND IsActive = 1;

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
                        SELECT EmployeeCode, 'First Name is required'
                        FROM EmployeeImportStaging
                        WHERE BatchId = @BatchId AND (FirstName IS NULL OR FirstName = '');

                        -- Auto-create missing designations
                        INSERT INTO DesignationMasters (Id, Name, IsActive, IsDeleted, CreatedOn)
                        SELECT DISTINCT NEWID(), s.Designation, 1, 0, GETUTCDATE()
                        FROM EmployeeImportStaging s
                        WHERE s.BatchId = @BatchId AND s.Designation IS NOT NULL AND s.Designation != ''
                          AND NOT EXISTS (SELECT 1 FROM DesignationMasters d WHERE d.Name = s.Designation AND d.IsDeleted = 0);

                        -- Auto-create missing practices
                        INSERT INTO Practices (Id, Name, IsActive, IsDeleted, CreatedOn)
                        SELECT DISTINCT NEWID(), s.Practice, 1, 0, GETUTCDATE()
                        FROM EmployeeImportStaging s
                        WHERE s.BatchId = @BatchId AND s.Practice IS NOT NULL AND s.Practice != ''
                          AND NOT EXISTS (SELECT 1 FROM Practices p WHERE p.Name = s.Practice AND p.IsDeleted = 0);

                        -- Auto-create missing locations
                        INSERT INTO Locations (Id, Name, Address, IsActive, IsDeleted, CreatedOn)
                        SELECT DISTINCT NEWID(), s.Location, s.Location, 1, 0, GETUTCDATE()
                        FROM EmployeeImportStaging s
                        WHERE s.BatchId = @BatchId AND s.Location IS NOT NULL AND s.Location != ''
                          AND NOT EXISTS (SELECT 1 FROM Locations l WHERE l.Name = s.Location AND l.IsDeleted = 0);

                        -- Auto-create missing work models
                        INSERT INTO WorkModelMasters (Id, Name, IsActive, IsDeleted, CreatedOn)
                        SELECT DISTINCT NEWID(), s.WorkModel, 1, 0, GETUTCDATE()
                        FROM EmployeeImportStaging s
                        WHERE s.BatchId = @BatchId AND s.WorkModel IS NOT NULL AND s.WorkModel != ''
                          AND NOT EXISTS (SELECT 1 FROM WorkModelMasters w WHERE w.Name = s.WorkModel AND w.IsDeleted = 0);

                        -- Auto-create missing employment types
                        INSERT INTO EmploymentTypeMasters (Id, Name, IsActive, IsDeleted, CreatedOn)
                        SELECT DISTINCT NEWID(), s.EmployeeType, 1, 0, GETUTCDATE()
                        FROM EmployeeImportStaging s
                        WHERE s.BatchId = @BatchId AND s.EmployeeType IS NOT NULL AND s.EmployeeType != ''
                          AND NOT EXISTS (SELECT 1 FROM EmploymentTypeMasters e WHERE e.Name = s.EmployeeType AND e.IsDeleted = 0);

                        -- Auto-create missing clients
                        INSERT INTO Clients (Name, StatusId, IsDeleted, CreatedOn)
                        SELECT DISTINCT s.Client, @DefaultStatusId, 0, GETUTCDATE()
                        FROM EmployeeImportStaging s
                        WHERE s.BatchId = @BatchId AND s.Client IS NOT NULL AND s.Client != ''
                          AND NOT EXISTS (SELECT 1 FROM Clients c WHERE c.Name = s.Client AND c.IsDeleted = 0);

                        -- Auto-create missing onboarding types
                        INSERT INTO OnboardingTypeMasters (Id, Name, IsActive, IsDeleted, CreatedOn)
                        SELECT DISTINCT NEWID(), s.Onboarding, 1, 0, GETUTCDATE()
                        FROM EmployeeImportStaging s
                        WHERE s.BatchId = @BatchId AND s.Onboarding IS NOT NULL AND s.Onboarding != ''
                          AND NOT EXISTS (SELECT 1 FROM OnboardingTypeMasters o WHERE o.Name = s.Onboarding AND o.IsDeleted = 0);

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

                        BEGIN TRANSACTION;

                        SELECT @DeletedRows = COUNT(1) FROM Employees WHERE IsDeleted = 0;

                        DELETE FROM EmployeeSkills;
                        DELETE FROM ResourceAllocations;
                        DELETE FROM Employees;

                        INSERT INTO Employees (
                            EmployeeCode, FirstName, LastName, FullName, Email,
                            DOJ, ExperienceYears, PriorExperience, RelevantExperience,
                            DesignationId, ClientId, OnboardingTypeId,
                            EmploymentTypeId, LocationId, WorkModelId, PracticeId,
                            DepartmentTypeId, StatusId,
                            MobileNumber, IsDeleted, CreatedOn, CreatedBy
                        )
                        SELECT
                            s.EmployeeCode,
                            s.FirstName,
                            s.LastName,
                            LTRIM(RTRIM(ISNULL(s.FirstName, '') + ' ' + ISNULL(s.LastName, ''))),
                            s.Email,
                            ISNULL(s.DOJ, GETUTCDATE()),
                            s.Experience,
                            ISNULL(s.Experience, 0),
                            s.Experience,
                            dm.Id,
                            c.Id,
                            ot.Id,
                            etm.Id,
                            l.Id,
                            wm.Id,
                            p.Id,
                            ISNULL(@DefaultDeptTypeId, (SELECT TOP 1 Id FROM DepartmentTypeMasters WHERE IsDeleted = 0 AND IsActive = 1)),
                            ISNULL(@DefaultStatusId, (SELECT TOP 1 Id FROM StatusMasters WHERE IsDeleted = 0 AND IsActive = 1)),
                            s.PhoneNumber,
                            0,
                            GETUTCDATE(),
                            @ImportedBy
                        FROM EmployeeImportStaging s
                        LEFT JOIN Practices p ON p.Name = s.Practice AND p.IsDeleted = 0
                        LEFT JOIN DesignationMasters dm ON dm.Name = s.Designation AND dm.IsDeleted = 0
                        LEFT JOIN Locations l ON l.Name = s.Location AND l.IsDeleted = 0
                        LEFT JOIN WorkModelMasters wm ON wm.Name = s.WorkModel AND wm.IsDeleted = 0
                        LEFT JOIN EmploymentTypeMasters etm ON etm.Name = s.EmployeeType AND etm.IsDeleted = 0
                        LEFT JOIN Clients c ON c.Name = s.Client AND c.IsDeleted = 0
                        LEFT JOIN OnboardingTypeMasters ot ON ot.Name = s.Onboarding AND ot.IsDeleted = 0
                        WHERE s.BatchId = @BatchId;

                        SET @ImportedRows = @@ROWCOUNT;

                        -- Create new skills that don't exist in master
                        INSERT INTO Skills (Id, Name, IsActive, IsDeleted, CreatedOn)
                        SELECT DISTINCT NEWID(), LTRIM(RTRIM(value)), 1, 0, GETUTCDATE()
                        FROM EmployeeImportStaging s
                        CROSS APPLY STRING_SPLIT(s.Skills, ',')
                        WHERE s.BatchId = @BatchId
                          AND s.Skills IS NOT NULL AND s.Skills != ''
                          AND LTRIM(RTRIM(value)) != ''
                          AND NOT EXISTS (
                            SELECT 1 FROM Skills sk
                            WHERE LTRIM(RTRIM(sk.Name)) = LTRIM(RTRIM(value)) AND sk.IsDeleted = 0
                          );

                        -- Insert EmployeeSkills
                        INSERT INTO EmployeeSkills (EmployeeId, SkillId)
                        SELECT DISTINCT e.Id, sk.Id
                        FROM EmployeeImportStaging s
                        INNER JOIN Employees e ON e.EmployeeCode = s.EmployeeCode
                        CROSS APPLY STRING_SPLIT(s.Skills, ',')
                        INNER JOIN Skills sk ON LTRIM(RTRIM(sk.Name)) = LTRIM(RTRIM(value)) AND sk.IsDeleted = 0
                        WHERE s.BatchId = @BatchId
                          AND s.Skills IS NOT NULL AND s.Skills != ''
                          AND LTRIM(RTRIM(value)) != ''
                          AND NOT EXISTS (
                            SELECT 1 FROM EmployeeSkills es
                            WHERE es.EmployeeId = e.Id AND es.SkillId = sk.Id
                          );

                        -- Clean up staging
                        DELETE FROM EmployeeImportStaging WHERE BatchId = @BatchId;

                        COMMIT TRANSACTION;

                        SELECT @TotalRows AS TotalRows, @ImportedRows AS ImportedRows, 0 AS FailedRows, @DeletedRows AS DeletedRows;

                    END TRY
                    BEGIN CATCH
                        IF @@TRANCOUNT > 0
                            ROLLBACK TRANSACTION;

                        DELETE FROM EmployeeImportStaging WHERE BatchId = @BatchId;

                        SELECT @TotalRows AS TotalRows, 0 AS ImportedRows, @TotalRows AS FailedRows, 0 AS DeletedRows;
                    END CATCH;

                    DROP TABLE #Errors;
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_ProcessEmployeeImport");
            migrationBuilder.Sql("DROP TABLE IF EXISTS EmployeeImportHistory");
            migrationBuilder.Sql("DROP TABLE IF EXISTS EmployeeImportStaging");
        }
    }
}
