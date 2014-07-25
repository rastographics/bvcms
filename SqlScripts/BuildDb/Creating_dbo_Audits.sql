CREATE TABLE [dbo].[Audits]
(
[Id] [int] NOT NULL IDENTITY(1, 1),
[Action] [nvarchar] (20) NOT NULL,
[TableName] [nvarchar] (100) NOT NULL,
[TableKey] [int] NULL,
[UserName] [nvarchar] (50) NOT NULL,
[AuditDate] [smalldatetime] NOT NULL
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
