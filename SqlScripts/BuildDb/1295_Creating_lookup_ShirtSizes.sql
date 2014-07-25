CREATE TABLE [lookup].[ShirtSizes]
(
[Id] [int] NOT NULL,
[Code] [nvarchar] (10) NULL,
[Description] [nvarchar] (50) NULL,
[Hardwired] [bit] NULL
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
