CREATE TABLE [dbo].[MobileAppIconSets]
(
[id] [int] NOT NULL IDENTITY(1, 1),
[name] [nvarchar] (100) NOT NULL CONSTRAINT [DF_MobileAppIconSets_name] DEFAULT (''),
[active] [bit] NOT NULL CONSTRAINT [DF_MobileAppIconSets_active] DEFAULT ((0))
) ON [PRIMARY]
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
