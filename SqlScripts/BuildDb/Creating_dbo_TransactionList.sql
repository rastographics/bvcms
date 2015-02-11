
CREATE VIEW [dbo].[TransactionList]
AS

SELECT *
FROM dbo.[Transaction] t
JOIN dbo.TransactionBalances tb ON tb.BalancesId = t.Id
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
