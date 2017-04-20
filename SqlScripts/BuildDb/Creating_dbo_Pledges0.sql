CREATE FUNCTION [dbo].[Pledges0]
(
	@fd DATETIME, 
	@td DATETIME,
	@fundid INT,
	@campusid INT
)
RETURNS 
@t TABLE ( PeopleId INT NOT NULL )
AS
BEGIN
	DECLARE @cc TABLE (PeopleId INT)

	INSERT INTO @cc
	SELECT CreditGiverId
	FROM Contributions2(@fd, @td, @campusid, 1, NULL, 1)
    WHERE (ISNULL(@fundid, 0) = 0 OR FundId = @fundid)
    AND PledgeAmount > 0
	GROUP BY CreditGiverId

	UNION

	SELECT CreditGiverId2
	FROM Contributions2(@fd, @td, @campusid, 1, NULL, 1)
    WHERE (ISNULL(@fundid,0) = 0 OR FundId = @fundid)
    AND PledgeAmount > 0
	AND CreditGiverId2 IS NOT NULL
	GROUP BY CreditGiverId2

	INSERT @t ( PeopleId )
	SELECT p.PeopleId 
	FROM dbo.People p
	LEFT JOIN @cc c ON c.PeopleId = p.PeopleId
	WHERE c.PeopleId IS NULL
	
	RETURN 
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
