

CREATE VIEW [dbo].[AllStatusFlags]
AS
	SELECT p1.PeopleId, sfr.Flag, sfr.Name, sfr.Role
	FROM dbo.People p1
	JOIN dbo.TagPerson tp ON p1.PeopleId = tp.PeopleId
	JOIN dbo.Tag t ON tp.Id = t.Id
	JOIN dbo.StatusFlagNamesRoles sfr ON sfr.Flag = SUBSTRING(t.Name, 1, 5)


GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
