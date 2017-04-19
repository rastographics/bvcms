CREATE FUNCTION [dbo].[GetPledgedTotalsBothIfJoint](@startdt DATETIME, @enddt DATETIME, @fundid INT)
RETURNS TABLE 
AS
RETURN 
(
	SELECT
		p.FamilyId,
		tt.PeopleId, 
		p.Name2 Name,
		SUM(tt.PledgeAmount) PledgeAmount,
		SUM(tt.Amount) Amount,
		SUM(tt.Balance) Balance

	FROM 
	(
		SELECT
			CreditGiverId PeopleId, 
			PledgeAmount,
			Amount,
			Balance
		FROM dbo.GetTotalPledgesDonor2(@startdt, @enddt, 0, @fundid)

		UNION
		SELECT
			CreditGiverId2 PeopleId, 
			PledgeAmount,
			Amount,
			Balance
		FROM dbo.GetTotalPledgesDonor2(@startdt, @enddt, 0, @fundid)
		WHERE CreditGiverId2 IS NOT NULL
	) tt
	JOIN dbo.People p ON p.PeopleId = tt.PeopleId
	GROUP BY p.FamilyId, tt.PeopleId, p.Name2
)



GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
