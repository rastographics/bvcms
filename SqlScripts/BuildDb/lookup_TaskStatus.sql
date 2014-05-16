CREATE TABLE [lookup].[TaskStatus]
(
[Id] [int] NOT NULL,
[Code] [nvarchar] (50) NULL,
[Description] [nvarchar] (100) NULL,
[Hardwired] [bit] NULL
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
