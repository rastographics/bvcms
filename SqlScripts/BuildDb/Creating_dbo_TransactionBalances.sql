
CREATE VIEW [dbo].[TransactionBalances]
AS
WITH trans AS (
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
			AND (Approved = 1 OR TransactionId LIKE 'Coupon%' OR (TransactionId LIKE 'zero due%' AND amtdue > 0))), 0)
			- ISNULL((SELECT SUM(Amt - ISNULL(donate,0)) 
				FROM dbo.[Transaction] 
				WHERE OriginalId = t.OriginalId 
				AND TransactionDate <= t.TransactionDate 
				AND amt IS NOT NULL
				AND ((Approved = 1 or TransactionId like 'Coupon%') or (TransactionId like 'zero due%' and amtdue > 0))), 0)
			TotDue
		,(SELECT COUNT(*) 
			FROM dbo.TransactionPeople
			WHERE Id = t.OriginalId) 
			NumPeople
		,ISNULL((SELECT STUFF((
				SELECT CHAR(10) + p.NAME + '(' + CONVERT(VARCHAR, ts.IndPctC * 100) + '%)'
				FROM dbo.TransactionSummary ts 
				JOIN dbo.People p ON p.PeopleId = ts.PeopleId
				WHERE ts.RegId = t.OriginalId
				FOR XML PATH(''),TYPE
				).value('text()[1]','nvarchar(max)'),1,1,N'' 
				))
			,(SELECT STUFF((
				SELECT CHAR(10) + p.NAME
				FROM dbo.TransactionPeople tp
				JOIN dbo.People p ON p.PeopleId = tp.PeopleId
				WHERE tp.Id = t.OriginalId
				FOR XML PATH(''),TYPE
				).value('text()[1]','nvarchar(max)'),1,1,N'' 
			  ))) 
		    People
		, t.batchtyp
		,CONVERT(BIT, 
			CASE WHEN ISNULL(t.Approved,0) = 1
				AND ISNULL(t.voided,0) != 1
				AND ISNULL(t.credited,0) != 1
				AND ISNULL(t.coupon, 0) = 0
				AND LEN(t.TransactionId) > 0
				AND LEN(t.TransactionGateway) > 0
				AND t.amt > 0
				THEN 1 ELSE 0 END)
			CanVoid
		,CONVERT(BIT, CASE 
				WHEN t.TransactionId LIKE 'Adjust%' THEN 1 
				WHEN t.TransactionId LIKE 'Payment%' THEN 1 
				WHEN t.TransactionId LIKE 'Initial Tran%' THEN 1 
				ELSE 0 
			END) IsAdjustment
	FROM dbo.[Transaction] t
)
SELECT BalancesId
		,BegBal
		,Payment
		,TotDue
		,NumPeople
		,People
		,CanVoid
		,CONVERT(BIT, 
			CASE WHEN CanVoid = 1 AND (Batchtyp = 'eft' OR Batchtyp = 'bankcard')
			THEN 1 ELSE 0 END) CanCredit
		,IsAdjustment
FROM trans




GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
