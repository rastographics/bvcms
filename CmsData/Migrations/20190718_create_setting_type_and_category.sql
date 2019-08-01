IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'DisplayName' AND Object_ID = Object_ID(N'dbo.Setting'))
    BEGIN
        ALTER TABLE [dbo].[Setting]
        ADD [DisplayName] varchar(MAX) NULL,
            [Description] varchar(MAX) NULL,
            [DataType] int NULL

        CREATE TABLE [dbo].[SettingType](
	        [SettingTypeId] [int] IDENTITY(1,1) NOT NULL,
	        [Name] [varchar](50) NOT NULL,
	        [DisplayOrder] [int] NOT NULL,
         CONSTRAINT [PK_SettingType] PRIMARY KEY CLUSTERED 
        (
	        [SettingTypeId] ASC
        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
        ) ON [PRIMARY]

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

        ALTER TABLE [dbo].[SettingCategory] CHECK CONSTRAINT [FK_SettingCategory_SettingType]

        ALTER TABLE [dbo].[Setting]
        ADD [SettingTypeId] [int] NULL

        ALTER TABLE [dbo].[Setting]  WITH CHECK ADD  CONSTRAINT [FK_Setting_SettingType] FOREIGN KEY([SettingTypeId])
        REFERENCES [dbo].[SettingType] ([SettingTypeId])

        ALTER TABLE [dbo].[Setting] CHECK CONSTRAINT [FK_Setting_SettingType]

        ALTER TABLE [dbo].[Setting]
        ADD [SettingCategoryId] [int] NULL

        ALTER TABLE [dbo].[Setting]  WITH CHECK ADD  CONSTRAINT [FK_Setting_SettingCategory] FOREIGN KEY([SettingCategoryId])
        REFERENCES [dbo].[SettingCategory] ([SettingCategoryId])

        ALTER TABLE [dbo].[Setting] CHECK CONSTRAINT [FK_Setting_SettingCategory]
    END
GO
