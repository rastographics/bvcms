CREATE PROCEDURE [dbo].[ClearFamilyExtraValuesForTag](@name VARCHAR(100), @tagid INT)
AS
BEGIN

	UPDATE dbo.FamilyExtra
	SET StrValue = NULL, DateValue = NULL, Data = NULL, IntValue = NULL, BitValue = NULL
	FROM dbo.FamilyExtra e
	JOIN dbo.People p ON p.FamilyId = e.FamilyId
	JOIN dbo.TagPerson tp ON tp.PeopleId = p.PeopleId AND tp.Id = @tagid
	WHERE e.Field = @name

END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
