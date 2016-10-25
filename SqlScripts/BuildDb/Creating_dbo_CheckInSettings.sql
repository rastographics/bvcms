CREATE TABLE [dbo].[CheckInSettings]
(
[id] [int] NOT NULL IDENTITY(1, 1),
[name] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_CheckInSettings_name] DEFAULT (''),
[settings] [nvarchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_CheckInSettings_settings] DEFAULT ('')
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
