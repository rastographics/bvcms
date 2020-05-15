IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'AppVersion'
          AND Object_ID = Object_ID(N'dbo.MobileAppDevices'))
BEGIN
	ALTER TABLE dbo.MobileAppDevices
	ADD [AppVersion] varchar(20)
END
GO
