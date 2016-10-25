
CREATE VIEW [dbo].[TransactionList]
AS

SELECT *
FROM dbo.[Transaction] t
JOIN dbo.TransactionBalances tb ON tb.BalancesId = t.Id
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
