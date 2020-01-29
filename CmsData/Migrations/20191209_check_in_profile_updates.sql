IF EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'EarlyCheckin'
          AND Object_ID = Object_ID(N'dbo.CheckinProfileSettings'))
BEGIN
	ALTER TABLE dbo.CheckinProfileSettings
	DROP COLUMN EarlyCheckin
    
	ALTER TABLE dbo.CheckinProfileSettings
	DROP COLUMN LateCheckin

    ALTER TABLE dbo.CheckinProfileSettings
    ALTER COLUMN [AdminPIN] NVARCHAR(10) NULL

    ALTER TABLE dbo.CheckinProfileSettings
    ALTER COLUMN Logout NVARCHAR(10) NULL
END
GO
