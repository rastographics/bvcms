CREATE FUNCTION [dbo].[Donors](@fd DATETIME, @td DATETIME, @pid INT, @spid INT, @fid INT, @noaddrok BIT, @tagid INT, @funds VARCHAR(MAX))
RETURNS TABLE 
AS
RETURN 
(
	SELECT	
			p.PrimaryAddress,
			p.PrimaryAddress2,
			p.PrimaryCity,
			p.PrimaryState,
			p.PrimaryZip,
			p.FamilyId,
			
			p.PeopleId, 
			p.LastName,
			ISNULL(p.FirstName, '') + ' ' + ISNULL(p.LastName, '') AS Name, 
			p.TitleCode AS Title, 
			p.SuffixCode AS Suffix,
			CASE 
				WHEN 2 IN (ISNULL(p.ContributionOptionsId, 0), ISNULL(sp.ContributionOptionsId,0)) 
						AND ISNULL(p.ContributionOptionsId,0) <> 9 THEN 2 
				WHEN ISNULL(p.ContributionOptionsId,0) = 2 
						AND (sp.PeopleId IS NULL OR ISNULL(sp.ContributionOptionsId, 0) = 9) THEN 1
				WHEN p.ContributionOptionsId IS NULL THEN 0
				ELSE p.ContributionOptionsId
			END AS ContributionOptionsId,
			p.DeceasedDate,
			p.Age,
			p.PositionInFamilyId,
			CASE 
				WHEN f.HeadOfHouseholdId = p.PeopleId THEN 1 
				WHEN f.HeadOfHouseholdSpouseId = p.PeopleId THEN 2 
				ELSE 3 
			END AS hohFlag,
		    ISNULL((SELECT SUM(ContributionAmount)
				FROM Contribution c 
				JOIN dbo.ContributionFund f ON c.FundId = f.FundId
				WHERE c.PeopleId = p.PeopleId
				AND ISNULL(f.NonTaxDeductible, 0) = 0
				AND (EXISTS(SELECT NULL FROM dbo.Setting WHERE Id = 'DisplayNonTaxOnStatement' AND Setting = 'true') OR c.ContributionTypeId <> 9)
				AND (@funds IS NULL OR c.FundId IN (SELECT Value FROM dbo.SplitInts(@funds)))
				AND c.ContributionStatusId = 0
				AND c.ContributionTypeId NOT IN (6,7)
				AND c.ContributionDate >= @fd
				AND c.ContributionDate < DATEADD(hh, 24, @td)), 0
			) AS Amount,
		    ISNULL((SELECT COUNT(*)
				FROM Contribution c 
				JOIN dbo.ContributionFund f ON c.FundId = f.FundId
				WHERE c.PeopleId = p.PeopleId
				AND ISNULL(f.NonTaxDeductible, 0) = 0
				AND (EXISTS(SELECT NULL FROM dbo.Setting WHERE Id = 'DisplayNonTaxOnStatement' AND Setting = 'true') OR c.ContributionTypeId <> 9)
				AND (@funds IS NULL OR c.FundId IN (SELECT Value FROM dbo.SplitInts(@funds)))
				AND c.ContributionStatusId = 0
				AND c.ContributionTypeId NOT IN (6,7)
				AND c.ContributionDate >= @fd
				AND c.ContributionDate < DATEADD(hh, 24, @td)), 0
			) AS GiftCount,
			CAST( CASE WHEN EXISTS(
				SELECT NULL FROM dbo.Contribution c
				JOIN dbo.ContributionFund f ON c.FundId = f.FundId
				WHERE c.PeopleId = p.PeopleId 
					-- NonTaxDeductible Fund or Pledge OR GiftInkind
					AND (ISNULL(f.NonTaxDeductible, 0) = 0 OR c.ContributionTypeId IN (8,10)) 
					AND (@funds IS NULL OR c.FundId IN (SELECT Value FROM dbo.SplitInts(@funds)))
					AND c.ContributionStatusId = 0
					AND c.ContributionTypeId NOT IN (6,7)
					AND c.ContributionDate >= @fd
					AND c.ContributionDate < DATEADD(hh, 24, @td)) 
				THEN 1 ELSE 0 END AS BIT
			) AS GiftInKind,
			
			sp.NAME AS SpouseName,
			sp.TitleCode AS SpouseTitle,
			p.SpouseId,
			CASE 
				WHEN 2 IN (ISNULL(p.ContributionOptionsId,0), ISNULL(sp.ContributionOptionsId,0)) 
						AND p.ContributionOptionsId <> 9 THEN 2 
				WHEN ISNULL(p.ContributionOptionsId,0) = 2 
						AND (sp.PeopleId IS NULL OR ISNULL(sp.ContributionOptionsId,0) = 9) THEN 1
				WHEN sp.ContributionOptionsId IS NULL THEN 0
				ELSE sp.ContributionOptionsId
				END AS SpouseContributionOptionsId,
		    ISNULL((SELECT SUM(ISNULL(ContributionAmount,0))
				FROM Contribution c 
				WHERE c.PeopleId = p.SpouseId
				AND (@funds IS NULL OR c.FundId IN (SELECT Value FROM dbo.SplitInts(@funds)))
				AND c.ContributionStatusId = 0
				AND c.ContributionTypeId NOT IN (6,7)
				AND c.ContributionDate >= @fd
				AND c.ContributionDate < DATEADD(hh, 24, @td)),0
			) AS SpouseAmount,
			p.CampusId,
			(SELECT LastName FROM dbo.People WHERE PeopleId = f.HeadOfHouseholdId
			) AS HouseName,
			ISNULL(p.ElectronicStatement, 0) ElectronicStatement,
			(SELECT Data FROM dbo.FamilyExtra WHERE Field = 'MailingAddress' AND FamilyId = f.FamilyId) MailingAddress,
			(SELECT Data FROM dbo.FamilyExtra WHERE Field = 'CoupleName' AND FamilyId = f.FamilyId) CoupleName
			
	from People p
	JOIN dbo.Families f ON p.FamilyId = f.FamilyId
	LEFT OUTER JOIN dbo.People sp ON p.SpouseId = sp.PeopleId
	
	WHERE 
	(@noaddrok = 1 OR 
		(p.PrimaryAddress <> '' 
		AND p.PrimaryBadAddrFlag = 0 
		AND p.DoNotMailFlag = 0 
		AND ISNULL(p.ContributionOptionsId, 0) <> 9))
	AND (@pid = 0 OR @pid = p.PeopleId OR @spid = p.PeopleId)
	AND (@fid = 0 OR @fid = p.FamilyId)
	AND (@tagid IS NULL OR EXISTS(SELECT NULL FROM TagPerson WHERE Id = @tagid AND PeopleId IN (p.PeopleId, sp.PeopleId)))
)


GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
