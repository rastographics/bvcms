CREATE TABLE [dbo].[MobileAppActionTypes]
(
[id] [int] NOT NULL IDENTITY(1, 1),
[name] [nvarchar] (50) NOT NULL CONSTRAINT [DF_MobileAppActionTypes_name] DEFAULT (''),
[loginType] [int] NOT NULL CONSTRAINT [DF_MobileAppActionTypes_requireLogin] DEFAULT ((0))
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
