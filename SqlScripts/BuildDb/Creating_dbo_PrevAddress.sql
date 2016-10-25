CREATE VIEW [dbo].[PrevAddress] AS 
	WITH lastele AS (
		SELECT d.PeopleId, d.Field, MAX(Created) Created
		FROM dbo.ChangeLogDetails d
		JOIN dbo.People p ON p.PeopleId = d.PeopleId
		WHERE Section = 'FamilyAddr'
		GROUP BY d.PeopleId, d.Field
	),
	previous AS (
		SELECT lastele.PeopleId, lastele.Field, Prev = ISNULL(d.before, '')
		FROM lastele
		JOIN dbo.ChangeLogDetails d ON d.PeopleId = lastele.PeopleId 
									AND d.Created = lastele.Created 
									AND d.Section = 'FamilyAddr'
									AND d.Field = lastele.Field
		WHERE LEN(ISNULL(d.Before, '')) > 0
	),
	grouped AS (
		SELECT
			PeopleId,
			MAX(CASE WHEN Field = 'AddressLineOne' THEN Prev END) PrevAddr,
			MAX(CASE WHEN Field = 'AddressLineTwo' THEN Prev END) PrevAddr2,
			MAX(CASE WHEN Field = 'CityName' THEN Prev END) PrevCity,
			MAX(CASE WHEN Field = 'StateCode' THEN Prev END) PrevState,
			MAX(CASE WHEN Field = 'ZipCode' THEN Prev END) PrevZip
		FROM previous
		GROUP BY previous.PeopleId
	),
	joined AS (
		SELECT g.PeopleId
              ,ISNULL(NULLIF(g.PrevAddr, ''), p.PrimaryAddress) PrevAddr
              ,ISNULL(NULLIF(g.PrevAddr2, ''), p.PrimaryAddress2) PrevAddr2
              ,ISNULL(NULLIF(g.PrevCity, ''), p.PrimaryCity) PrevCity
              ,ISNULL(NULLIF(g.PrevState, ''), p.PrimaryState) PrevState
              ,ISNULL(NULLIF(g.PrevZip, ''), p.PrimaryZip) PrevZip
		FROM grouped g
		JOIN dbo.People p ON p.PeopleId = g.PeopleId
	)
	SELECT joined.PeopleId
          ,joined.PrevAddr
          ,joined.PrevAddr2
          ,joined.PrevCity
          ,joined.PrevState
          ,joined.PrevZip
	FROM joined


GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
