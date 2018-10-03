IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'IsDeleted'
          AND Object_ID = Object_ID(N'dbo.SMSGroups'))
BEGIN
    ALTER TABLE dbo.SMSGroups ADD IsDeleted BIT NOT NULL DEFAULT 0
END
GO
