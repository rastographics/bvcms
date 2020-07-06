IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'code'
          AND Object_ID = Object_ID(N'dbo.Users'))
BEGIN
    ALTER TABLE dbo.Users
    ADD [code] [varchar] (64) DEFAULT NULL,
    [codeExpires] [datetime] DEFAULT NULL,
    [codeEmail] [varchar] (255) DEFAULT NULL
END
GO
