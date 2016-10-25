CREATE TABLE [dbo].[MobileAppIconSets]
(
[id] [int] NOT NULL IDENTITY(1, 1),
[name] [nvarchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_MobileAppIconSets_name] DEFAULT (''),
[active] [bit] NOT NULL CONSTRAINT [DF_MobileAppIconSets_active] DEFAULT ((0))
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
