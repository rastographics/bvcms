
CREATE PROC [dbo].[CleanOrgFilters]
AS
BEGIN
	DECLARE @t TABLE (id UNIQUEIDENTIFIER)
	INSERT @t (id) SELECT QueryId FROM dbo.OrgFilter WHERE LastUpdated < DATEADD(HOUR, -24, GETDATE())

	DELETE dbo.OrgFilter
	FROM dbo.OrgFilter f
	JOIN @t ON [@t].id = f.QueryId

	DELETE dbo.Query
	FROM dbo.Query q
	JOIN @t ON id = q.QueryId

	DELETE dbo.TagPerson
	FROM dbo.TagPerson tp
	JOIN dbo.Tag t ON t.Id = tp.Id AND t.TypeId = 3
	JOIN @t ON [@t].id = t.Name

	DELETE dbo.Tag
	FROM dbo.Tag t
	JOIN @t ON [@t].Id = t.Name
	WHERE TypeId = 3

	DELETE dbo.LongRunningOperation
	WHERE completed < DATEADD(HOUR, -24, GETDATE())
END




GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
