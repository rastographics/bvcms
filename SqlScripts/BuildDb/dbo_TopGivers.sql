CREATE PROCEDURE [dbo].[TopGivers](@top INT, @sdate DATETIME, @edate DATETIME)
AS
BEGIN

	SELECT TOP (@top) c.PeopleId, Name, SUM(ContributionAmount) FROM dbo.People p
	JOIN dbo.Contribution c ON p.PeopleId = c.PeopleId
	WHERE c.ContributionDate >= @sdate
	AND c.ContributionDate <= @edate
	AND c.ContributionTypeId NOT IN (6,7,8)
	GROUP BY c.PeopleId, Name
	ORDER BY SUM(ContributionAmount) DESC

END
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
