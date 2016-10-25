


CREATE VIEW [dbo].[DonorProfileList]
AS
SELECT 
	PeopleId,
	SpouseId,
	FamilyId,
	FORMAT(marjnt,'')
	+ ',' + FORMAT(agerng, '')
	+ ',' + FORMAT(yearsrng, '')
	+ ',' + FORMAT(amt13rng, '')
	+ ',' + FORMAT(totyear1_rng, '')
	+ ',' + FORMAT(totyear2_rng, '') prof
FROM
(
	SELECT 
		ISNULL(MaritalStatusId + ContributionOptionsId, 100) marjnt,
		FLOOR(ISNULL(Age, 0) / 10.0)*10 agerng,
		CEILING(nyears / 3.0)*3 yearsrng,
		CEILING(ISNULL(amt13, 0) / 5000.0)*5000 amt13rng,
		CEILING(ISNULL(amttot, 0) / (CASE WHEN nyears > 0 THEN nyears ELSE 1 END) / 5000.0)*5000 totyear1_rng,
		CEILING(ISNULL(amttotsp, 0) / (CASE WHEN nyears > 0 THEN nyears ELSE 1 END) / 5000.0)*5000 totyear2_rng,
		t2.* 
	FROM
	(
		SELECT 
			p.PeopleId, 
			p.SpouseId, 
			p.FamilyId, 
			Age,
			MaritalStatusId,
			ISNULL(p.ContributionOptionsId, 0) ContributionOptionsId,
			(SELECT SUM(ISNULL(ContributionAmount, 0)) 
				FROM dbo.Contribution 
				WHERE PeopleId = p.PeopleId)
				amttot,
			(SELECT SUM(ISNULL(ContributionAmount, 0)) 
				FROM dbo.Contribution 
				WHERE PeopleId = f.HeadOfHouseholdSpouseId)
				amttotsp,
			(SELECT SUM(ISNULL(ContributionAmount,0)) 
				FROM dbo.Contribution 
				WHERE PeopleId = p.PeopleId 
				AND DATEPART(YEAR, ContributionDate) = 2013)
				amt13,
			(SELECT COUNT(*) 
				FROM dbo.Contribution 
				WHERE PeopleId = p.PeopleId 
				AND DATEPART(YEAR, ContributionDate) = 2013)
				cnt13,
			(SELECT COUNT(*) 
				FROM dbo.Contribution c
				JOIN dbo.People sp ON sp.PeopleId = c.PeopleId
				JOIN dbo.People hp ON hp.PeopleId = sp.SpouseId 
				WHERE hp.PeopleId = p.PeopleId 
				AND DATEPART(YEAR, ContributionDate) = 2013)
				spousecnt,
			(SELECT COUNT(*) FROM
				(
					SELECT COUNT(*) cnt
					FROM dbo.Contribution
					WHERE PeopleId = p.PeopleId
					GROUP BY DATEPART(YEAR, ContributionDate)
				) tt ) nyears
		FROM dbo.People p
		JOIN dbo.Families f ON f.FamilyId = p.FamilyId
		WHERE (EXISTS (SELECT NULL 
						FROM dbo.Contribution 
						WHERE PeopleId = p.PeopleId)
				OR EXISTS(SELECT NULL 
							FROM dbo.Contribution 
							WHERE PeopleId = f.HeadOfHouseholdSpouseId) )
		AND p.PeopleId = f.HeadOfHouseholdId
	) t2
) t3



GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
