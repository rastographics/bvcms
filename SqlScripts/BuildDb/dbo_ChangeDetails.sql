CREATE TABLE [dbo].[ChangeDetails]
(
[Id] [int] NOT NULL,
[Field] [nvarchar] (50) NOT NULL,
[Before] [nvarchar] (max) NULL,
[After] [nvarchar] (max) NULL
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
