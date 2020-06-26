IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'code'
          AND Object_ID = Object_ID(N'dbo.Users'))
BEGIN
    ALTER TABLE dbo.Users
    ADD [code] [varchar] (64) NOT NULL CONSTRAINT code_default DEFAULT (''),
    [codeExpires] [datetime] NOT NULL CONSTRAINT codeExpires_default DEFAULT ('1970-01-01 00:00:00'),
    [codeEmail] [varchar] (255) NOT NULL CONSTRAINT codeEmail_default DEFAULT ('')
END
GO
