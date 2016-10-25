CREATE FUNCTION [dbo].[FirstTimeGivers] ( @days INT, @fundid INT )
RETURNS TABLE 
AS
RETURN 
	SELECT PeopleId, FirstDate, Amt
	FROM
	(
		SELECT CreditGiverId, SUM(Amount) AS Amt, MIN(Date) as FirstDate
		FROM dbo.Contributions2('1/1/1900', GETDATE(), 0, 0, 0, 1) c
		WHERE c.FundId = @fundid OR @fundid = 0
		GROUP BY CreditGiverId
	) tt
	JOIN dbo.People p ON p.PeopleId = tt.CreditGiverId
	WHERE FirstDate > DATEADD(dd, -@days, GETDATE())
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
