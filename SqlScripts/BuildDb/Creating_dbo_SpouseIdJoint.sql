CREATE FUNCTION [dbo].[SpouseIdJoint] 
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
	AND s.ContributionOptionsId = 2
	AND p.ContributionOptionsId = 2
	
	RETURN @Result

END

GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
