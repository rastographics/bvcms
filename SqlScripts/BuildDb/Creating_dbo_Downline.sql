CREATE TABLE [dbo].[Downline]
(
[OrgId] [int] NULL,
[LeaderId] [int] NULL,
[DiscipleId] [int] NULL,
[StartDt] [datetime] NULL,
[EndDt] [datetime] NULL
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
