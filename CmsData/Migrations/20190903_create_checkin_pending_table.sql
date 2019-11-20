IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES where 
	TABLE_NAME = 'CheckInPending' AND 
	TABLE_SCHEMA = 'dbo')
	BEGIN	
        CREATE TABLE [dbo].[CheckInPending](
	        [Id] [int] IDENTITY NOT NULL PRIMARY KEY,
	        [Stamp] [datetime] NOT NULL,
	        [Data] [nvarchar](max) NULL
            )
	END
GO
