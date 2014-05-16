






CREATE VIEW [dbo].[TransactionSummary]
AS
SELECT
	 RegId
	,OrganizationId
	,PeopleId
	,TranDate
	,IndAmt
	,TotalAmt
	,TotPaid
	,(TotalAmt - TotPaid) TotDue
	,(IndPctC * TotPaid) IndPaid
	,(IndPctC * (TotalAmt - TotPaid)) IndDue
	,NumPeople
	,isdeposit
	,iscoupon
	,Donation

FROM (
	SELECT 
		ot.TransactionDate TranDate
		,tp.Amt IndAmt
		,tp.Amt / NULLIF((SELECT SUM(Amt) 
							FROM dbo.TransactionPeople 
							WHERE Id = om.TranId), 0)
			IndPctC
		,-ISNULL((SELECT SUM(Amt - ISNULL(donate, 0)) 
					FROM dbo.[Transaction] 
					WHERE OriginalId = om.TranId 
					AND Amt < 0), 0) 
			+ ot.amt + ot.amtdue 
			TotalAmt
		,(SELECT SUM(amt - ISNULL(donate, 0)) 
			FROM dbo.[Transaction] 
			WHERE OriginalId = om.TranId 
			AND (Approved = 1 OR TransactionId LIKE 'Coupon%')
			AND Amt > 0) 
			TotPaid
		,(SELECT COUNT(*) 
			FROM dbo.TransactionPeople t 
			WHERE t.Id = om.TranId) 
			NumPeople
		,ot.OriginalId RegId
		,o.OrganizationId
		,tp.PeopleId
		,CONVERT(BIT, CASE WHEN ot.amt < ot.amtdue THEN 1 ELSE 0 END)
			isdeposit
		,ot.Approved isapproved
		,CONVERT(BIT, CASE WHEN ot.TransactionId LIKE 'Coupon%' THEN 1 ELSE 0 END)
			iscoupon
		,ISNULL((SELECT SUM(donate) 
					FROM dbo.[Transaction] 
					WHERE OriginalId = om.TranId AND donate > 0), 0) 
			Donation
	FROM dbo.TransactionPeople tp
	JOIN dbo.[Transaction] ot ON ot.Id = tp.Id
	JOIN dbo.OrganizationMembers om ON om.TranId = ot.Id AND tp.PeopleId = om.PeopleId
	JOIN dbo.Organizations o ON o.OrganizationId = om.OrganizationId
) tt
WHERE (iscoupon = 1 OR isapproved = 1) AND TotalAmt > 0





GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
