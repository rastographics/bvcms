IF NOT EXISTS(select 1 from sys.tables where name = 'SessionValues')
CREATE TABLE dbo.SessionValues
	(
	[SessionId] nvarchar(88) NOT NULL,
	[Name] nvarchar(88) NOT NULL,
	[Value] nvarchar(MAX) NULL,
    [CreatedDate] datetime NOT NULL DEFAULT (GETDATE())
	)
    TEXTIMAGE_ON [PRIMARY]
GO

IF NOT EXISTS(select 1 from sys.key_constraints where parent_object_id = OBJECT_ID('dbo.SessionValues') AND [type] = 'PK')
    ALTER TABLE dbo.SessionValues ADD CONSTRAINT PK_SessionValues PRIMARY KEY CLUSTERED ( [SessionId], [Name] ) 
    WITH ( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY] 
GO

ALTER TABLE dbo.SessionValues SET (LOCK_ESCALATION = TABLE)
GO