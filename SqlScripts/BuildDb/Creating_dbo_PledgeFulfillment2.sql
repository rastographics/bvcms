
CREATE PROC [dbo].[PledgeFulfillment2] ( @fundid1 INT, @fundid2 INT )
AS
BEGIN
	SELECT 
			CreditGiverId 
			, c.SpouseId
			, c.FamilyId
			, FundId
			, Date
			, PledgeAmount
			, Amount
			INTO #t
		FROM Contributions2('1/1/1', '1/1/3000', 0, NULL, NULL, 1) c 
		WHERE c.FundId IN (@fundid1, @fundid2)

	SELECT tt.firstname
		,tt.LastName
		,tt.spouse
		,tt.memberstatus

		,c1.PledgeDate PledgeDate1
		,c2.PledgeDate PledgeDate2

		,c1.LastDate LastDate1
		,c2.LastDate LastDate2

		,c1.PledgeAmt PledgeAmt1
		,c2.PledgeAmt PledgeAmt2

		,c1.TotalGiven TotalGiven1
		,c2.TotalGiven TotalGiven2

		,c1.Balance Balance1
		,c2.Balance Balance2

		,tt.CreditGiverId
		,tt.SpouseId
		,tt.FamilyId
	FROM (
		SELECT 
			p.PreferredName firstname
			,p.LastName
			,sp.PreferredName spouse
			,ms.Description memberstatus
			,c.CreditGiverId
			,c.SpouseId
			,c.FamilyId
		FROM #t c
		JOIN dbo.People p ON c.CreditGiverId = p.PeopleId
		JOIN lookup.MemberStatus ms ON p.MemberStatusId = ms.Id
		LEFT OUTER JOIN dbo.People sp ON p.SpouseId = sp.PeopleId
		WHERE c.FundId IN (@fundid1, @fundid2)
		GROUP BY p.PeopleId, c.CreditGiverId, c.SpouseId, p.PreferredName, p.LastName, sp.PreferredName, ms.Description, c.FamilyId
	) tt
	LEFT JOIN (
		SELECT CreditGiverId
		,(SELECT MIN(Date) FROM #t 
			WHERE PledgeAmount > 0 
			AND FundId = @fundid1 
			AND CreditGiverId = ct.CreditGiverId
		) PledgeDate
		,MAX(Date) LastDate
		,SUM(PledgeAmount) PledgeAmt
		,SUM(Amount) TotalGiven
		,CASE WHEN SUM(ISNULL(PledgeAmount, 0)) > 0 
				THEN SUM(ISNULL(PledgeAmount, 0)) - SUM(ISNULL(Amount, 0))
				ELSE 0
		 END Balance
		FROM #t ct
		WHERE FundId = @fundid1
		GROUP BY CreditGiverId
	) c1 ON c1.CreditGiverId = tt.CreditGiverId
	LEFT JOIN (
		SELECT CreditGiverId
		,(SELECT MIN(Date) FROM #t 
			WHERE PledgeAmount > 0 
			AND FundId = @fundid2 
			AND CreditGiverId = ct.CreditGiverId
		) PledgeDate
		,MAX(Date) LastDate
		,SUM(PledgeAmount) PledgeAmt
		,SUM(Amount) TotalGiven
		,CASE WHEN SUM(ISNULL(PledgeAmount, 0)) > 0 
				THEN SUM(ISNULL(PledgeAmount, 0)) - SUM(ISNULL(Amount, 0))
				ELSE 0
		 END Balance
		FROM #t ct
		WHERE FundId = @fundid2
		GROUP BY CreditGiverId
	) c2 ON c2.CreditGiverId = tt.CreditGiverId

	DROP TABLE #t

END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
