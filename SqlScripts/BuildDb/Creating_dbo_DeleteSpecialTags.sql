CREATE PROCEDURE [dbo].[DeleteSpecialTags](@pid INT = null)
AS
BEGIN
	SET NOCOUNT ON;
	EXEC SetupNumbers

	DELETE dbo.TagPerson
	FROM dbo.TagPerson tp
	JOIN dbo.Tag t ON tp.Id = t.Id
	WHERE t.TypeId >= 3 AND t.TypeId < 100 
	AND (t.PeopleId = @pid OR @pid IS NULL)

	DELETE FROM dbo.Tag
	WHERE TypeId >= 3 AND TypeId < 100
	AND (PeopleId = @pid OR @pid IS NULL)

	DELETE dbo.TagShare
	FROM dbo.TagShare s
	JOIN dbo.Tag t ON s.TagId = t.Id
	WHERE t.TypeId = 2 -- System
	AND t.Name LIKE '.temp email tag%'
	AND (t.PeopleId = @pid OR @pid IS NULL)
	AND t.Created < DATEADD(HOUR, -16, GETDATE())

	DELETE dbo.TagPerson
	FROM dbo.TagPerson tp
	JOIN dbo.Tag t ON tp.Id = t.Id
	WHERE t.TypeId = 2 -- System
	AND t.Name LIKE '.temp email tag%'
	AND (t.PeopleId = @pid)
	AND t.Created < DATEADD(HOUR, -16, GETDATE())

	DELETE FROM dbo.Tag
	WHERE TypeId = 2 -- System
	AND Name LIKE '.temp email tag%'
	AND (PeopleId = @pid)
	AND Created < DATEADD(HOUR, -16, GETDATE())

	DELETE dbo.TagPerson
	FROM dbo.TagPerson tp
	JOIN dbo.Tag t ON tp.Id = t.Id
	WHERE t.TypeId = 2 -- System
	AND t.Name = 'FromTrackBirthdaysQuery' 
	AND (t.PeopleId = @pid)

	DELETE FROM dbo.Tag
	WHERE TypeId = 2 -- System
	AND Name = 'FromTrackBirthdaysQuery' 
	AND (PeopleId = @pid)

	DELETE dbo.TagPerson
	FROM dbo.TagPerson tp
	JOIN dbo.Tag t ON tp.Id = t.Id
	WHERE t.TypeId = 1 AND t.PeopleId IS NULL

	DELETE dbo.TagShare
	FROM dbo.TagShare s
	JOIN dbo.Tag t ON s.TagId = t.Id
	WHERE TypeId = 1 AND t.PeopleId IS NULL

	DELETE FROM dbo.Tag
	WHERE TypeId = 1 AND PeopleId IS NULL

END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
