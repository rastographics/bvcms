IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES where 
	TABLE_NAME = 'CheckinProfiles' AND 
	TABLE_SCHEMA = 'dbo')
	BEGIN				
		CREATE TABLE [dbo].[CheckinProfiles](
			[CheckinProfileId][int] IDENTITY(1,1) PRIMARY KEY,
			[Name] NVARCHAR(100) NOT NULL UNIQUE
		);
	END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES where 
	TABLE_NAME = 'CheckinProfileSettings' AND 
	TABLE_SCHEMA = 'dbo')
	BEGIN				
		CREATE TABLE [dbo].CheckinProfileSettings(
			[CheckinProfileId][int] FOREIGN KEY REFERENCES [dbo].[CheckinProfiles](CheckinProfileId) NOT NULL UNIQUE,
			[CampusId][int] NULL,
			[EarlyCheckin][int] NULL,
			[LateCheckin][int] NULL,
			[Testing] BIT NOT NULL,
			[TestDay][int] NULL,
			[AdminPIN] NVARCHAR(MAX) NULL,
			[PINTimeout][int] NULL,
			[DisableJoin] BIT NOT NULL,
			[DisableTimer] BIT NOT NULL,
			[BackgroundImage][int] NULL,
			[BackgroundImageName] NVARCHAR(MAX) NULL, 
			[BackgroundImageURL] NVARCHAR(MAX) NULL, 
			[CutoffAge][int] NOT NULL,
			[Logout] NVARCHAR(MAX) NULL,
			[Guest] BIT NOT NULL,
			[Location] BIT NOT NULL,
			[SecurityType][int] NOT NULL,
			[ShowCheckinConfirmation][int] NOT NULL
		);
	END
GO
