CREATE TABLE [dbo].[DownlineLeaders]
(
[CategoryId] [int] NULL,
[PeopleId] [int] NULL,
[Name] [nvarchar] (100) NULL,
[Cnt] [int] NULL,
[Levels] [int] NULL
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
