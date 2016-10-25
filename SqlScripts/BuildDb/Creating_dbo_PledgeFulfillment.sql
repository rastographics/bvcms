CREATE FUNCTION [dbo].[PledgeFulfillment] ( @fundid INT )
RETURNS TABLE 
AS
RETURN 
(
	SELECT 
		p.PreferredName [First]
		,p.LastName [Last]
		,sp.PreferredName Spouse
		,ms.Description [MemberStatus]
		,(SELECT MIN(Date) FROM Contributions2('1/1/1', '1/1/3000', 0, NULL, NULL, 1) WHERE FundId = @fundid AND CreditGiverId = c.CreditGiverId AND PledgeAmount > 0) PledgeDate
		,MAX(Date) LastDate
		,SUM(PledgeAmount) PledgeAmt
		,SUM(Amount) TotalGiven
		,CASE WHEN SUM(ISNULL(c.PledgeAmount, 0)) > 0 
				THEN SUM(ISNULL(PledgeAmount, 0)) - SUM(ISNULL(Amount, 0))
				ELSE 0
		 END Balance
		,p.PrimaryAddress [Address]
		,p.PrimaryCity [City]
		,p.PrimaryState [State]
		,p.PrimaryZip [Zip]
		,c.CreditGiverId
		,c.SpouseId
		,c.FamilyId
	FROM Contributions2('1/1/1', '1/1/3000', 0, NULL, NULL, 1) c
	JOIN dbo.People p ON c.CreditGiverId = p.PeopleId
	JOIN lookup.MemberStatus ms ON p.MemberStatusId = ms.Id
	LEFT OUTER JOIN dbo.People sp ON p.SpouseId = sp.PeopleId
	WHERE c.FundId = @fundid
	GROUP BY c.CreditGiverId, c.SpouseId, p.PreferredName, p.LastName, sp.PreferredName, p.PrimaryAddress, p.PrimaryCity, p.PrimaryState, p.PrimaryZip, ms.Description, c.FamilyId
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
