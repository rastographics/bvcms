CREATE FUNCTION [dbo].[RecentGiverFund](@days INT, @fundid INT)
RETURNS TABLE 
AS
RETURN 
(
	WITH contributions AS (
		SELECT *
		FROM Contributions2(
				DATEADD(DAY, -@days, GETDATE()),
				GETDATE(),
				0, 0, 0, 1)
		WHERE Amount > 0
		AND FundId = @fundid
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
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
