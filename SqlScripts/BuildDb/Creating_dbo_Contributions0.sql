CREATE FUNCTION [dbo].[Contributions0]
(
	@fd DATETIME, 
	@td DATETIME,
	@fundid INT,
	@campusid INT,
	@pledges BIT,
	@nontaxded BIT,
	@includeUnclosed BIT
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT PeopleId FROM dbo.People 
	WHERE PeopleId NOT IN 
	(
		SELECT CreditGiverId
		FROM Contributions2(@fd, @td, @campusid, @pledges, @nontaxded, @includeUnclosed)
        WHERE (ISNULL(@fundid, 0) = 0 OR FundId = @fundid)
        AND Amount > 0
		GROUP BY CreditGiverId
	)
	AND PeopleId NOT IN
	(
		SELECT SpouseId
		FROM Contributions2(@fd, @td, @campusid, @pledges, @nontaxded, @includeUnclosed)
        WHERE (ISNULL(@fundid, 0) = 0 OR FundId = @fundid)
        AND Amount > 0
		AND SpouseId IS NOT NULL
		GROUP BY CreditGiverId, SpouseId
	)
)

GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
