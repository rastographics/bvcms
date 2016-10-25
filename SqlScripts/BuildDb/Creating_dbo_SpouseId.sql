CREATE FUNCTION [dbo].[SpouseId] 
(
	@peopleid int
)
RETURNS int
AS
BEGIN
	DECLARE @Result int

	SELECT TOP 1 @Result = s.PeopleId FROM dbo.People p
	JOIN dbo.People s ON s.FamilyId = p.FamilyId
	WHERE s.PeopleId <> @peopleid AND p.PeopleId = @peopleid
	AND p.MaritalStatusId = 20
	AND s.MaritalStatusId = 20
	AND s.DeceasedDate IS NULL
	AND p.DeceasedDate IS NULL
	AND p.PositionInFamilyId = s.PositionInFamilyId
	AND s.FirstName <> 'Duplicate'	-- Return the result of the function
	
	RETURN @Result

END

GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
