CREATE TABLE [dbo].[MobileAppActions]
(
[id] [int] NOT NULL IDENTITY(1, 1),
[type] [int] NOT NULL CONSTRAINT [DF_MobileAppActions_type] DEFAULT ((0)),
[title] [nvarchar] (50) NOT NULL CONSTRAINT [DF_MobileAppActions_title] DEFAULT (''),
[option] [int] NOT NULL CONSTRAINT [DF_MobileAppActions_option] DEFAULT ((0)),
[data] [nvarchar] (max) NOT NULL CONSTRAINT [DF_MobileAppActions_url] DEFAULT (''),
[order] [int] NOT NULL CONSTRAINT [DF_MobileAppActions_order] DEFAULT ((0)),
[loginType] [int] NOT NULL CONSTRAINT [DF_MobileAppActions_loginType] DEFAULT ((0)),
[enabled] [bit] NOT NULL CONSTRAINT [DF_MobileAppActions_enabled] DEFAULT ((1)),
[roles] [nvarchar] (max) NOT NULL CONSTRAINT [DF_MobileAppActions_roles] DEFAULT (''),
[api] [int] NOT NULL CONSTRAINT [DF_MobileAppActions_api] DEFAULT ((0))
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
