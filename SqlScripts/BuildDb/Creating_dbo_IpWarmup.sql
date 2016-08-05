CREATE TABLE [dbo].[IpWarmup]
(
[epoch] [datetime] NULL,
[sentsince] [int] NULL,
[since] [datetime] NULL,
[totalsent] [int] NULL,
[totaltries] [int] NULL
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
