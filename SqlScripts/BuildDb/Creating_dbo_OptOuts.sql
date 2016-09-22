CREATE FUNCTION [dbo].[OptOuts](@queueid INT, @fromemail VARCHAR(100))
RETURNS TABLE 
AS
RETURN 
(
	SELECT 
		p.PeopleId,
		p.Name,
		OptOut = CAST(IIF(EXISTS(SELECT NULL FROM dbo.EmailOptOut WHERE ToPeopleId = p.PeopleId AND FromEmail = @fromemail), 1, 0) AS BIT),
		HhPeopleId = h1.PeopleId,
		HhName = h1.Name,
		HhEmail = h1.EmailAddress,
		HhSpPeopleId = h2.PeopleId,
		HhSpName = h2.Name,
		HhSpEmail = h2.EmailAddress
	FROM dbo.People p
	JOIN dbo.Families f ON f.FamilyId = p.FamilyId
	LEFT JOIN dbo.People h1 ON h1.PeopleId = f.HeadOfHouseholdId AND p.PositionInFamilyId = 30
		AND NOT EXISTS(SELECT NULL FROM dbo.EmailOptOut WHERE ToPeopleId = h1.PeopleId AND FromEmail = @fromemail)
	LEFT JOIN dbo.People h2 ON h2.PeopleId = f.HeadOfHouseholdSpouseId AND p.PositionInFamilyId = 30
		AND NOT EXISTS(SELECT NULL FROM dbo.EmailOptOut WHERE ToPeopleId = h2.PeopleId AND FromEmail = @fromemail)
	WHERE (@queueid = 0 OR EXISTS(SELECT NULL FROM dbo.EmailQueueTo WHERE PeopleId = p.PeopleId AND Id = @queueid))
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
