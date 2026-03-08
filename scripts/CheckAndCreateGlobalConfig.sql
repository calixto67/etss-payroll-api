-- Check if GlobalConfig table exists (run against ETSSPayrollDb)
-- Server: 192.168.100.64,1433\SQLEXPRESS

-- 1) Check existence
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'GlobalConfig')
BEGIN
    PRINT 'Table GlobalConfig does NOT exist. Creating it...';

    CREATE TABLE [dbo].[GlobalConfig] (
        [Id]          INT             NOT NULL IDENTITY(1,1),
        [ConfigName]  NVARCHAR(128)   NOT NULL,
        [ConfigValue] VARBINARY(MAX)  NOT NULL,
        [MimeType]    NVARCHAR(128)   NULL,
        [CreatedDate] DATETIME2       NOT NULL,
        [CreatedBy]   NVARCHAR(MAX)   NOT NULL,
        [UpdatedDate] DATETIME2       NULL,
        [UpdatedBy]   NVARCHAR(MAX)   NULL,
        CONSTRAINT [PK_GlobalConfig] PRIMARY KEY ([Id])
    );

    CREATE UNIQUE INDEX [IX_GlobalConfig_ConfigName] ON [dbo].[GlobalConfig] ([ConfigName]);

    PRINT 'Table GlobalConfig created.';
END
ELSE
    PRINT 'Table GlobalConfig already exists.';

-- 2) Rename old columns CreatedAt -> CreatedDate, UpdatedAt -> UpdatedDate
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.GlobalConfig') AND name = 'CreatedAt')
BEGIN
    EXEC sp_rename 'dbo.GlobalConfig.CreatedAt', 'CreatedDate', 'COLUMN';
    PRINT 'Renamed CreatedAt -> CreatedDate.';
END

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.GlobalConfig') AND name = 'UpdatedAt')
BEGIN
    EXEC sp_rename 'dbo.GlobalConfig.UpdatedAt', 'UpdatedDate', 'COLUMN';
    PRINT 'Renamed UpdatedAt -> UpdatedDate.';
END

-- 3) Add CreatedDate/UpdatedDate columns if they don't exist at all
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'GlobalConfig')
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.GlobalConfig') AND name = 'CreatedDate')
    BEGIN
        ALTER TABLE [dbo].[GlobalConfig] ADD [CreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE();
        PRINT 'Added CreatedDate column.';
    END

    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.GlobalConfig') AND name = 'UpdatedDate')
    BEGIN
        ALTER TABLE [dbo].[GlobalConfig] ADD [UpdatedDate] DATETIME2 NULL;
        PRINT 'Added UpdatedDate column.';
    END
END

-- 4) Show table info
SELECT
    t.name AS TableName,
    (SELECT COUNT(*) FROM [dbo].[GlobalConfig]) AS RowCount
FROM sys.tables t
WHERE t.name = 'GlobalConfig';
