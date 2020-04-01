ALTER function [dbo].[RecentGiverFunds](@days int, @funds varchar(max), @nontaxded bit)
RETURNS TABLE 
AS
RETURN 
(
	WITH contributions AS (
		SELECT *
		FROM Contributions2(
				DATEADD(DAY, -@days, GETDATE()),
				GETDATE(),
				0, 0, @nontaxded, 1, null)
		WHERE Amount > 0
		AND FundId in (select value from dbo.SplitInts(@funds))
	), BOTH AS (
		SELECT contributions.CreditGiverId PeopleId
		FROM contributions

		UNION

		SELECT contributions.SpouseId 
		FROM contributions
	)
	SELECT DISTINCT PeopleId FROM BOTH
	WHERE PeopleId IS NOT NULL
)
