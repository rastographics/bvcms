








CREATE VIEW [dbo].[TransactionSummary]
AS
SELECT
	 RegId
	,OrganizationId
	,PeopleId
	,TranDate
	,CONVERT(MONEY, TotalAmt * IndPctC) IndAmt
	,TotalAmt
	,TotPaid
	,TotCoupon
	,(TotalAmt - TotPaid - Donation) TotDue
	,CONVERT(MONEY, (IndPctC * TotPaid)) IndPaid
	,CONVERT(MONEY, (IndPctC * (TotalAmt - TotPaid - Donation))) IndDue
	,IndPctC
	,NumPeople
	,isdeposit
	,iscoupon
	,Donation

FROM (
	SELECT 
		ot.TransactionDate TranDate

		,ISNULL(CONVERT(FLOAT,tp.Amt) 
			/ NULLIF((SELECT SUM(Amt) 
					FROM dbo.TransactionPeople tp2
					JOIN dbo.OrganizationMembers om2 
					ON om2.PeopleId = tp2.PeopleId AND om2.OrganizationId = tp2.OrgId
					WHERE Id = om.TranId), 0), 0)
			IndPctC

		,-ISNULL((SELECT SUM(Amt - ISNULL(donate, 0)) 
					FROM dbo.[Transaction] 
					WHERE OriginalId = om.TranId 
					AND ISNULL(AdjustFee, 0) = 1), 0) 
			+ ot.amt + ot.amtdue 
			TotalAmt

		,ISNULL((SELECT SUM(amt - ISNULL(donate, 0)) 
			FROM dbo.[Transaction] 
			WHERE OriginalId = om.TranId 
			AND (Approved = 1 OR TransactionId LIKE 'Coupon%')
			AND ISNULL(AdjustFee, 0) = 0), 0) 
			TotPaid

		,ISNULL((SELECT SUM(amt) 
			FROM dbo.[Transaction] 
			WHERE OriginalId = om.TranId 
			AND TransactionId LIKE 'Coupon%'), 0)
			TotCoupon

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
	JOIN dbo.OrganizationMembers om ON om.TranId = ot.Id AND tp.PeopleId = om.PeopleId AND om.OrganizationId = tp.OrgId
	JOIN dbo.Organizations o ON o.OrganizationId = om.OrganizationId
) tt
--WHERE (iscoupon = 1 OR isapproved = 1) --AND TotalAmt > 0










GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
