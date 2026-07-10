-- ============================================================
-- HRMS User Management Refactoring: Role String -> RoleId (FK)
-- ============================================================
-- This script:
-- 1. Adds RoleId column to Users table
-- 2. Creates FK_Users_RoleMasters foreign key
-- 3. Migrates existing text-based Role values to RoleId
-- 4. Drops the old Role column
-- 5. Creates index on RoleId
-- ============================================================

BEGIN TRANSACTION;
SET XACT_ABORT ON;

-- Step 1: Add RoleId column (nullable initially for migration)
ALTER TABLE [dbo].[Users] ADD [RoleId] uniqueidentifier NULL;

-- Step 2: Migrate existing data
-- Map each text Role value to the corresponding RoleMaster Id
UPDATE u
SET u.RoleId = r.Id
FROM [dbo].[Users] u
INNER JOIN [dbo].[RoleMasters] r ON r.[Name] = u.[Role] AND r.[IsDeleted] = 0;

-- If any users still have NULL RoleId (e.g., role name not found),
-- assign them the "Employee" role (77777777-7777-7777-7777-777777777777)
UPDATE [dbo].[Users]
SET RoleId = '77777777-7777-7777-7777-777777777777'
WHERE RoleId IS NULL;

-- Step 3: Make RoleId NOT NULL
ALTER TABLE [dbo].[Users] ALTER COLUMN [RoleId] uniqueidentifier NOT NULL;

-- Step 4: Create Foreign Key constraint (RESTRICT delete)
ALTER TABLE [dbo].[Users] ADD CONSTRAINT [FK_Users_RoleMasters_RoleId]
FOREIGN KEY ([RoleId]) REFERENCES [dbo].[RoleMasters] ([Id])
ON DELETE NO ACTION;

-- Step 5: Drop the old Role column
ALTER TABLE [dbo].[Users] DROP COLUMN [Role];

-- Step 6: Create index on RoleId for performance
CREATE NONCLUSTERED INDEX [IX_Users_RoleId] ON [dbo].[Users] ([RoleId]);

COMMIT TRANSACTION;
GO
