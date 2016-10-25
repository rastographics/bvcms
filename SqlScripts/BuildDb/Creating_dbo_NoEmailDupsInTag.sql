CREATE PROCEDURE [dbo].[NoEmailDupsInTag] (@tagid INT)
AS
BEGIN
		(SELECT PeopleId INTO #t
		FROM (SELECT p.PeopleId, ROW_NUMBER() 
				OVER(PARTITION BY EmailAddress ORDER BY EmailAddress) row
		      FROM dbo.People p
			  JOIN dbo.TagPerson pp ON pp.PeopleId = p.PeopleId AND pp.Id = @tagid
		     ) tt 
		WHERE row = 1)

	DELETE dbo.TagPerson
	WHERE Id = @tagid
	AND PeopleId NOT IN (SELECT PeopleId FROM #t)

	DROP TABLE #t
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
