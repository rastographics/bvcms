CREATE FUNCTION [dbo].[LastAddrElement](@ele VARCHAR(20))
RETURNS TABLE
AS
RETURN
(
	WITH lastele AS (
		SELECT d.PeopleId, MAX(Created) Created
		FROM dbo.ChangeLogDetails d
		JOIN dbo.People p ON p.PeopleId = d.PeopleId
		WHERE Field = @ele
		AND Section = 'FamilyAddr'
		AND ISNULL(NULLIF(After, '(null)'), '') = 
			CASE @ele
				WHEN 'AddressLineOne' THEN ISNULL(p.PrimaryAddress, '')
				WHEN 'AddressLineTwo' THEN ISNULL(p.PrimaryAddress2, '')
				WHEN 'CityName' THEN ISNULL(p.PrimaryCity, '')
				WHEN 'StateCode' THEN ISNULL(p.PrimaryState, '')
				WHEN 'ZipCode' THEN ISNULL(p.ZipCode, '')
				ELSE NULL
			END
		GROUP BY d.PeopleId
	)
	SELECT lastele.PeopleId, Prev = ISNULL(d.before, '')
	FROM lastele
	JOIN dbo.ChangeLogDetails d ON d.PeopleId = lastele.PeopleId 
								AND lastele.Created = d.Created 
								AND d.Section = 'FamilyAddr'
								AND d.Field = @ele
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
