CREATE PROCEDURE [dbo].[ClearExtraValuesForTag](@name VARCHAR(100), @tagid INT)
AS
BEGIN

	UPDATE dbo.PeopleExtra
	SET StrValue = NULL, DateValue = NULL, Data = NULL, IntValue = NULL, IntValue2 = NULL, BitValue = NULL
	FROM dbo.PeopleExtra e
	JOIN dbo.TagPerson tp ON tp.PeopleId = e.PeopleId AND tp.Id = @tagid
	WHERE e.Field = @name

END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
