IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE Object_ID = Object_ID(N'dbo.SettingType'))
    BEGIN
		CREATE TABLE [dbo].[SettingType](
	        [SettingTypeId] [int] IDENTITY(1,1) NOT NULL,
	        [Name] [varchar](50) NOT NULL,
	        [DisplayOrder] [int] NOT NULL,
         CONSTRAINT [PK_SettingType] PRIMARY KEY CLUSTERED 
        (
	        [SettingTypeId] ASC
        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
        ) ON [PRIMARY]
    END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE Object_ID = Object_ID(N'dbo.SettingCategory'))
    BEGIN
		CREATE TABLE [dbo].[SettingCategory](
	        [SettingCategoryId] [int] IDENTITY(1,1) NOT NULL,
	        [Name] [nvarchar](50) NOT NULL,
	        [SettingTypeId] [int] NOT NULL,
	        [DisplayOrder] [int] NOT NULL,
         CONSTRAINT [PK_SettingCategory] PRIMARY KEY CLUSTERED 
        (
	        [SettingCategoryId] ASC
        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
        ) ON [PRIMARY]
        ALTER TABLE [dbo].[SettingCategory]  WITH CHECK ADD  CONSTRAINT [FK_SettingCategory_SettingType] FOREIGN KEY([SettingTypeId])
        REFERENCES [dbo].[SettingType] ([SettingTypeId])
    END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE Object_ID = Object_ID(N'dbo.SettingMetadata'))
    BEGIN
		CREATE TABLE [dbo].[SettingMetadata](
		    [SettingId] [nvarchar](50) NOT NULL,
		    [DisplayName] [varchar](max) NULL,
		    [Description] [varchar](max) NULL,
		    [DataType] [int] NULL,
		    [SettingTypeId] [int] NULL,
		    [SettingCategoryId] [int] NULL,
		CONSTRAINT [PK_SettingMetadata] PRIMARY KEY CLUSTERED 
		(
			[SettingId] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
		ALTER TABLE [dbo].[SettingMetadata]  WITH CHECK ADD  CONSTRAINT [FK_SettingMetadata_SettingCategory] FOREIGN KEY([SettingCategoryId])
			REFERENCES [dbo].[SettingCategory] ([SettingCategoryId])
		ALTER TABLE [dbo].[SettingMetadata]  WITH CHECK ADD  CONSTRAINT [FK_SettingMetadata_SettingType] FOREIGN KEY([SettingTypeId])
			REFERENCES [dbo].[SettingType] ([SettingTypeId])
        ALTER TABLE [dbo].[SettingMetadata]  WITH CHECK ADD  CONSTRAINT [FK_SettingMetadata_Setting] FOREIGN KEY([SettingId])
			REFERENCES [dbo].[Setting] ([Id])

        INSERT INTO SettingMetadata (SettingId)
        SELECT Id
        FROM Setting 
    END
GO
