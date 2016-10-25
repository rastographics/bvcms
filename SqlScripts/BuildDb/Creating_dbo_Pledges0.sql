CREATE FUNCTION [dbo].[Pledges0]
(
	@fd DATETIME, 
	@td DATETIME,
	@fundid INT,
	@campusid INT
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT PeopleId FROM dbo.People 
	WHERE PeopleId NOT IN 
	(
		SELECT CreditGiverId
		FROM Contributions2(@fd, @td, @campusid, 1, NULL, 1)
        WHERE (ISNULL(@fundid, 0) = 0 OR FundId = @fundid)
        AND PledgeAmount > 0
		GROUP BY CreditGiverId
	)
	AND PeopleId NOT IN
	(
		SELECT SpouseId
		FROM Contributions2(@fd, @td, @campusid, 1, NULL, 1)
        WHERE (ISNULL(@fundid,0) = 0 OR FundId = @fundid)
        AND PledgeAmount > 0
		AND SpouseId IS NOT NULL
		GROUP BY CreditGiverId, SpouseId
	)
)


GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
