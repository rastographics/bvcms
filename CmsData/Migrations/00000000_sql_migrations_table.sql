IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON (t.schema_id = s.schema_id)
WHERE s.name = 'dbo' AND t.name = '__SqlMigrations')
BEGIN
    CREATE TABLE dbo.__SqlMigrations
	(
        Id nvarchar(80) NOT NULL PRIMARY KEY CLUSTERED,
        CreatedDate datetime2(7) NOT NULL DEFAULT (GETDATE())
	)
END
GO
