ALTER PROCEDURE [dbo].[TopGivers](@top INT, @sdate DATETIME, @edate DATETIME, @fundids VARCHAR(MAX))
AS
BEGIN

	--SELECT TOP (@top) c.PeopleId, Name, SUM(ContributionAmount) FROM dbo.People p
	--JOIN dbo.Contribution c ON p.PeopleId = c.PeopleId
	--WHERE c.ContributionDate >= @sdate
	--AND c.ContributionDate <= @edate
	--AND c.ContributionTypeId NOT IN (6,7,8)
	--GROUP BY c.PeopleId, Name
	--ORDER BY SUM(ContributionAmount) DESC

	SELECT TOP (@top)
	c.CreditGiverId PeopleId, HeadName Name, SUM(Amount) Amount
    FROM dbo.Contributions2(@sdate, @edate, 0, 0, NULL, 1, @fundids) c 
    GROUP BY c.CreditGiverId, c.HeadName
    ORDER BY SUM(c.Amount) DESC

END
GO