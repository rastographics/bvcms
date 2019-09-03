IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES where 
	TABLE_NAME = 'CheckInPending' AND 
	TABLE_SCHEMA = 'dbo')
	BEGIN	
        CREATE TABLE [dbo].[CheckInPending](
	        [Id] [nvarchar](50) NOT NULL,
	        [Stamp] [datetime] NOT NULL,
	        [Data] [nvarchar](max) NULL,
         CONSTRAINT [PK_Data] PRIMARY KEY CLUSTERED 
        (
	        [Id] ASC,
	        [Stamp] ASC
        ));
	END
GO
