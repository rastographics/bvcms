CREATE FUNCTION [dbo].[PersonStatusFlags] ( @tagid INT )
RETURNS TABLE 
AS
RETURN 
(
	WITH PidStatusFlag(PeopleId, StatusFlag)
	AS
	(
		SELECT p.PeopleId, ts.Name AS StatusFlag
		FROM dbo.People p
		JOIN dbo.TagPerson tp ON tp.PeopleId = p.PeopleId
		JOIN dbo.Tag t ON t.Id = tp.Id
		LEFT JOIN dbo.TagPerson tps ON p.PeopleId = tps.PeopleId
		LEFT JOIN Tag ts ON tps.Id = ts.Id
		WHERE ts.Name LIKE 'F[0-9][0-9]'
		AND t.Id = @tagid
	)
	SELECT PeopleId, StatusFlags =
	    STUFF((SELECT ', ' + StatusFlag
	           FROM PidStatusFlag b 
	           WHERE b.PeopleId = a.PeopleId 
	          FOR XML PATH('')), 1, 2, '')
	FROM PidStatusFlag a
	GROUP BY a.PeopleId
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
