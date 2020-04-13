ALTER PROCEDURE [dbo].[TopGivers](@top INT, @sdate DATETIME, @edate DATETIME, @fundids VARCHAR(MAX), @nontaxded bit)
AS
BEGIN
	SELECT TOP (@top)
	c.CreditGiverId PeopleId, HeadName Name, SUM(Amount) Amount
    FROM dbo.Contributions2(@sdate, @edate, 0, 0, @nontaxded, 1, @fundids) c 
    GROUP BY c.CreditGiverId, c.HeadName
    ORDER BY SUM(c.Amount) DESC

END
