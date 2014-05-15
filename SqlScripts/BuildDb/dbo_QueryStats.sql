CREATE TABLE [dbo].[QueryStats]
(
[RunId] [int] NOT NULL,
[StatId] [nvarchar] (5) NOT NULL,
[Runtime] [datetime] NOT NULL,
[Description] [nvarchar] (75) NOT NULL,
[Count] [int] NOT NULL
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
