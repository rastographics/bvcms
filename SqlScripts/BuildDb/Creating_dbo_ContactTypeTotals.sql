CREATE FUNCTION [dbo].[ContactTypeTotals](@dt1 DATETIME, @dt2 DATETIME, @min INT)
RETURNS TABLE 
AS
RETURN 
(
	SELECT t2.Id, t2.Description, t2.[Count], t1.CountPeople FROM
	(
		SELECT ContactTypeId, COUNT(*) [CountPeople] FROM
		(
		SELECT DISTINCT ContactTypeId, e.PeopleId
		FROM dbo.Contactees e 
		JOIN dbo.Contact c ON c.ContactId = e.ContactId
		WHERE (c.ContactDate >= @dt1 OR @dt1 IS NULL)
		AND (c.ContactDate <= @dt2 OR @dt2 IS NULL)
		AND (c.MinistryId = @min OR @min = 0)
		) tt
		GROUP BY tt.ContactTypeId
	) t1
	JOIN
	(
		SELECT Id, Description, COUNT(*) [Count]
		FROM dbo.Contact c
		JOIN lookup.ContactType t ON c.ContactTypeId = t.Id
		WHERE (c.ContactDate >= @dt1 OR @dt1 IS NULL)
		AND (c.ContactDate <= @dt2 OR @dt2 IS NULL)
		AND (c.MinistryId = @min OR @min = 0)
		GROUP BY t.Id, t.DESCRIPTION
	) t2 ON t1.ContactTypeId = t2.Id
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
