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
[api] [int] NOT NULL CONSTRAINT [DF_MobileAppActions_api] DEFAULT ((0)),
[active] [datetime] NOT NULL CONSTRAINT [DF_MobileAppActions_active] DEFAULT ('1970-01-01 12:00:00'),
[altTitle] [nvarchar] (50) NOT NULL CONSTRAINT [DF_MobileAppActions_altTitle] DEFAULT (''),
[rebranded] [int] NOT NULL CONSTRAINT [DF_MobileAppActions_rebranded] DEFAULT ((0))
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
