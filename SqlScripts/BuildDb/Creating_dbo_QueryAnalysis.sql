CREATE TABLE [dbo].[QueryAnalysis]
(
[Id] [uniqueidentifier] NOT NULL,
[Seconds] [int] NULL,
[OriginalCount] [int] NULL,
[ParsedCount] [int] NULL,
[Message] [varchar] (max) NULL
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
