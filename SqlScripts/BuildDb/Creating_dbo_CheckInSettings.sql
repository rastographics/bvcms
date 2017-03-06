CREATE TABLE [dbo].[CheckInSettings]
(
[id] [int] NOT NULL IDENTITY(1, 1),
[name] [nvarchar] (50) NOT NULL CONSTRAINT [DF_CheckInSettings_name] DEFAULT (''),
[settings] [nvarchar] (max) NOT NULL CONSTRAINT [DF_CheckInSettings_settings] DEFAULT ('')
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
