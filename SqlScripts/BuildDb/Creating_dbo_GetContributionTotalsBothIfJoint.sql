CREATE FUNCTION [dbo].[GetContributionTotalsBothIfJoint](@startdt DATETIME, @enddt DATETIME, @fundid INT)
RETURNS TABLE 
AS
RETURN 
(
	SELECT
		p.FamilyId,
		tt.PeopleId, 
		p.Name2 Name,
		SUM(tt.Amount) Amount

	FROM 
	(
		SELECT
			CreditGiverId PeopleId, 
			Amount
		FROM dbo.GetTotalContributionsDonor2(@startdt, @enddt, 0, 0, 1, NULL, @fundid)

		UNION
		SELECT
			CreditGiverId2 PeopleId, 
			Amount
		FROM dbo.GetTotalContributionsDonor2(@startdt, @enddt, 0, 0, 1, NULL, @fundid)
		WHERE CreditGiverId2 IS NOT NULL
	) tt
	JOIN dbo.People p ON p.PeopleId = tt.PeopleId
	GROUP BY p.FamilyId, tt.PeopleId, p.Name2
)



GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
