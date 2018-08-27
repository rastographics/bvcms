IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'SystemFlag'
          AND Object_ID = Object_ID(N'dbo.SMSNumbers'))
BEGIN
    ALTER TABLE dbo.SMSNumbers ADD SystemFlag bit NOT NULL CONSTRAINT DF_SMSNumbers_SystemFlag DEFAULT 0
END
