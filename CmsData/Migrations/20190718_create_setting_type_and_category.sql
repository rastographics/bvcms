CREATE TABLE [dbo].[SettingType](
	[SettingTypeId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[DisplayOrder] [int] NOT NULL,
 CONSTRAINT [PK_SettingType] PRIMARY KEY CLUSTERED 
(
	[SettingTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

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
GO

ALTER TABLE [dbo].[SettingCategory]  WITH CHECK ADD  CONSTRAINT [FK_SettingCategory_SettingType] FOREIGN KEY([SettingTypeId])
REFERENCES [dbo].[SettingType] ([SettingTypeId])
GO

ALTER TABLE [dbo].[SettingCategory] CHECK CONSTRAINT [FK_SettingCategory_SettingType]
GO

ALTER TABLE [dbo].[Setting]
ADD [SettingTypeId] [int] NULL
GO

ALTER TABLE [dbo].[Setting]  WITH CHECK ADD  CONSTRAINT [FK_Setting_SettingType] FOREIGN KEY([SettingTypeId])
REFERENCES [dbo].[SettingType] ([SettingTypeId])
GO

ALTER TABLE [dbo].[Setting] CHECK CONSTRAINT [FK_Setting_SettingType]
GO

ALTER TABLE [dbo].[Setting]
ADD [SettingCategoryId] [int] NULL
GO

ALTER TABLE [dbo].[Setting]  WITH CHECK ADD  CONSTRAINT [FK_Setting_SettingCategory] FOREIGN KEY([SettingCategoryId])
REFERENCES [dbo].[SettingCategory] ([SettingCategoryId])
GO

ALTER TABLE [dbo].[Setting] CHECK CONSTRAINT [FK_Setting_SettingCategory]
GO
