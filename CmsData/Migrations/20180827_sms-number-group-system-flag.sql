IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'SystemFlag'
          AND Object_ID = Object_ID(N'dbo.SMSGroups'))
BEGIN
    ALTER TABLE dbo.SMSGroups ADD SystemFlag bit NOT NULL CONSTRAINT DF_SMSGroups_SystemFlag DEFAULT 0
END
