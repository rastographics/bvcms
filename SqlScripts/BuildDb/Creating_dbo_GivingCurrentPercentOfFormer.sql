CREATE FUNCTION [dbo].[GivingCurrentPercentOfFormer]
(
	@dt1 DATETIME, 
	@dt2 DATETIME,
	@comp nvarchar(2),
	@pct FLOAT
)
RETURNS TABLE 
AS
RETURN 
(
SELECT pid, c1amt, c2amt, pct
FROM (
	SELECT pid, c1amt, c2amt, NULLIF(c2amt, 0) * 100 / NULLIF(c1amt, 0) pct
	FROM (	
		SELECT pid, SUM(c1amt) c1amt, SUM(c2amt) c2amt
		FROM (	
			SELECT p1.PeopleId pid, SUM(c1.Amount) c1amt, 0 c2amt
			FROM dbo.People p1
			JOIN dbo.Contributions2(@dt1, @dt2, 0, 0, 0, 0) c1 ON p1.PeopleId = c1.CreditGiverId
			GROUP BY p1.PeopleId
			UNION
			SELECT p2.PeopleId pid, 0 c1amt, SUM(c2.Amount) c2amt
			FROM dbo.People p2
			JOIN dbo.Contributions2(@dt2, GETDATE(), 0, 0, 0, 0) c2 ON p2.PeopleId = c2.CreditGiverId
			GROUP BY p2.PeopleId
		) t1
		GROUP BY t1.pid
	) t2
) t3
WHERE (@comp <> '<=' OR t3.pct <= @pct)
AND (@comp <> '>' OR t3.pct > @pct)
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
