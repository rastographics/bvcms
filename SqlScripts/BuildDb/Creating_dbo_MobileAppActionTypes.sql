CREATE TABLE [dbo].[MobileAppActionTypes]
(
[id] [int] NOT NULL IDENTITY(1, 1),
[name] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_MobileAppActionTypes_name] DEFAULT (''),
[loginType] [int] NOT NULL CONSTRAINT [DF_MobileAppActionTypes_requireLogin] DEFAULT ((0))
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
