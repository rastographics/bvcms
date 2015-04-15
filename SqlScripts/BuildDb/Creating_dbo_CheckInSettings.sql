CREATE TABLE [dbo].[CheckInSettings]
(
[id] [int] NOT NULL IDENTITY(1, 1),
[name] [nvarchar] (50) NOT NULL CONSTRAINT [DF_CheckInSettings_name] DEFAULT (''),
[settings] [nvarchar] (max) NOT NULL CONSTRAINT [DF_CheckInSettings_settings] DEFAULT ('')
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
