-- Run this ONLY if the table already exists with the OLD name "GlobalConfigs" and old columns
-- (Key, Value, IsDeleted, DeletedAt, DeletedBy). Alters to new schema and renames table to GlobalConfig.

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'GlobalConfigs')
BEGIN
    -- Rename Key -> ConfigName, Value -> ConfigValue (SQL Server)
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.GlobalConfigs') AND name = 'Key')
        EXEC sp_rename 'dbo.GlobalConfigs.[Key]', 'ConfigName', 'COLUMN';
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.GlobalConfigs') AND name = 'Value')
        EXEC sp_rename 'dbo.GlobalConfigs.[Value]', 'ConfigValue', 'COLUMN';

    -- Drop old index on Key if it exists
    IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('dbo.GlobalConfigs') AND name = 'IX_GlobalConfigs_Key')
        DROP INDEX [IX_GlobalConfigs_Key] ON [dbo].[GlobalConfigs];

    -- Create new unique index on ConfigName
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('dbo.GlobalConfigs') AND name = 'IX_GlobalConfig_ConfigName')
        CREATE UNIQUE INDEX [IX_GlobalConfig_ConfigName] ON [dbo].[GlobalConfigs] ([ConfigName]);

    -- Remove soft-delete columns if present
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.GlobalConfigs') AND name = 'IsDeleted')
        ALTER TABLE [dbo].[GlobalConfigs] DROP COLUMN [IsDeleted];
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.GlobalConfigs') AND name = 'DeletedAt')
        ALTER TABLE [dbo].[GlobalConfigs] DROP COLUMN [DeletedAt];
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.GlobalConfigs') AND name = 'DeletedBy')
        ALTER TABLE [dbo].[GlobalConfigs] DROP COLUMN [DeletedBy];

    -- Rename table from GlobalConfigs to GlobalConfig
    EXEC sp_rename 'dbo.GlobalConfigs', 'GlobalConfig';

    -- Add date columns if missing
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.GlobalConfig') AND name = 'CreatedDate')
        ALTER TABLE [dbo].[GlobalConfig] ADD [CreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE();
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.GlobalConfig') AND name = 'UpdatedDate')
        ALTER TABLE [dbo].[GlobalConfig] ADD [UpdatedDate] DATETIME2 NULL;

    PRINT 'Table altered and renamed to GlobalConfig.';
END
ELSE
    PRINT 'Table GlobalConfigs does not exist. Run CheckAndCreateGlobalConfig.sql to create GlobalConfig.';
