/*
Run this script on:

        108.166.25.29.CMS_testdb    -  This database will be modified

to synchronize it with:

        (local).CMS_bellevue

You are recommended to back up your database before running this script

Script created by SQL Compare version 10.7.0 from Red Gate Software Ltd at 10/23/2014 9:43:30 PM

*/
SET NUMERIC_ROUNDABORT OFF
GO
SET ANSI_PADDING, ANSI_WARNINGS, CONCAT_NULL_YIELDS_NULL, ARITHABORT, QUOTED_IDENTIFIER, ANSI_NULLS ON
GO
IF EXISTS (SELECT * FROM tempdb..sysobjects WHERE id=OBJECT_ID('tempdb..#tmpErrors')) DROP TABLE #tmpErrors
GO
CREATE TABLE #tmpErrors (Error int)
GO
SET XACT_ABORT ON
GO
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
GO
BEGIN TRANSACTION
GO
PRINT N'Altering [dbo].[TransactionBalances]'
GO

ALTER VIEW [dbo].[TransactionBalances]
AS
SELECT BalancesId
		,BegBal
		,Payment
		,TotDue
		,NumPeople
		,CanVoid
		,CONVERT(BIT, 
			CASE WHEN CanVoid = 1 AND (Batchtyp = 'eft' OR Batchtyp = 'bankcard')
			THEN 1 ELSE 0 END) CanCredit
FROM (
	SELECT 
		t.Id BalancesId
		,(SELECT amt + amtdue - ISNULL(donate, 0)
			FROM dbo.[Transaction] 
			WHERE Id = t.OriginalId)
			- ISNULL(( SELECT SUM(Amt) 
				FROM dbo.[Transaction] 
				WHERE OriginalId = t.OriginalId 
				AND TransactionDate < t.TransactionDate), 0)
			BegBal
		,t.amt - ISNULL(t.donate, 0) Payment
		,ISNULL((SELECT amt + ISNULL(amtdue, 0) - ISNULL(donate, 0)
			FROM dbo.[Transaction] 
			WHERE Id = t.OriginalId
			AND (Approved = 1 OR TransactionId LIKE 'Coupon%')), 0)
			- ISNULL((SELECT SUM(Amt - ISNULL(donate,0)) 
				FROM dbo.[Transaction] 
				WHERE OriginalId = t.OriginalId 
				AND TransactionDate <= t.TransactionDate 
				AND amt IS NOT NULL
				AND (Approved = 1 OR TransactionId LIKE 'Coupon%')), 0)
			TotDue
		,(SELECT COUNT(*) 
			FROM dbo.TransactionPeople
			WHERE Id = t.OriginalId) 
			NumPeople
		, t.batchtyp
		,CONVERT(BIT, 
			CASE WHEN ISNULL(t.Approved,0) = 1
				AND ISNULL(t.voided,0) != 1
				AND ISNULL(t.credited,0) != 1
				AND ISNULL(t.coupon, 0) = 0
				AND LEN(t.TransactionId) > 0
				AND t.amt > 0
				THEN 1 ELSE 0 END)
			CanVoid

	FROM dbo.[Transaction] t
) tt


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Refreshing [dbo].[TransactionList]'
GO
EXEC sp_refreshview N'[dbo].[TransactionList]'
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
IF EXISTS (SELECT * FROM #tmpErrors) ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT>0 BEGIN
PRINT 'The database update succeeded'
COMMIT TRANSACTION
END
ELSE PRINT 'The database update failed'
GO
DROP TABLE #tmpErrors
GO
