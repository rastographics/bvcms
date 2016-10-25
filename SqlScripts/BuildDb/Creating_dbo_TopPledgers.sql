CREATE PROCEDURE [dbo].[TopPledgers](@top INT, @sdate DATETIME, @edate DATETIME)
AS
BEGIN

	SELECT TOP (@top) c.PeopleId, Name, SUM(ContributionAmount) FROM dbo.People p
	JOIN dbo.Contribution c ON p.PeopleId = c.PeopleId
	WHERE c.ContributionDate >= @sdate
	AND c.ContributionDate <= @edate
	AND c.ContributionTypeId = 8
	GROUP BY c.PeopleId, Name
	ORDER BY SUM(ContributionAmount) DESC

END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
