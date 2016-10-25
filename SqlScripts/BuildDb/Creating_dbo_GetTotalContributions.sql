CREATE FUNCTION [dbo].[GetTotalContributions](@startdt DATETIME, @enddt DATETIME)
RETURNS TABLE 
AS
RETURN 
(
	SELECT 
		hh.PeopleId, 
		hh.Name, 
		hh.SpouseName, 
		f.FundId,
		f.FundName AS FundDescription,
		(SELECT COUNT(*) FROM dbo.Contribution cc
				JOIN dbo.People p ON cc.PeopleId = p.PeopleId
				WHERE cc.FundId = f.Fundid 
				AND ( (cc.PeopleId = hh.PeopleId)
				 OR (cc.PeopleId IN (hh.SpouseId, hh.PeopleId) AND hh.ContributionOptionsId = 2 AND hh.SpouseContributionOptionsId = 2)
					)
				AND cc.ContributionTypeId NOT IN (6,7)
				AND cc.ContributionStatusId = 0
				AND cc.ContributionDate >= @startdt 
				AND cc.ContributionDate < DATEADD(hh, 24, @enddt)
				) Cnt,
		(SELECT SUM(cc.ContributionAmount) FROM dbo.Contribution cc
				JOIN dbo.People p ON cc.PeopleId = p.PeopleId
				WHERE cc.FundId = f.Fundid 
				AND ( (cc.PeopleId = hh.PeopleId)
				 OR (cc.PeopleId IN (hh.SpouseId, hh.PeopleId) AND hh.ContributionOptionsId = 2 AND hh.SpouseContributionOptionsId = 2)
					)
				AND cc.ContributionTypeId NOT IN (6,7,8)
				AND cc.ContributionStatusId = 0
				AND cc.ContributionDate >= @startdt 
				AND cc.ContributionDate < DATEADD(hh, 24, @enddt)
				) Amt,
		(SELECT SUM(cc.ContributionAmount) FROM dbo.Contribution cc
				JOIN dbo.People p ON cc.PeopleId = p.PeopleId
				WHERE cc.FundId = f.Fundid 
				AND ( (cc.PeopleId = hh.PeopleId)
				 OR (cc.PeopleId IN (hh.SpouseId, hh.PeopleId) AND hh.ContributionOptionsId = 2 AND hh.SpouseContributionOptionsId = 2)
					)
				AND cc.ContributionTypeId = 8
				AND cc.ContributionStatusId = 0
				AND cc.ContributionDate >= @startdt 
				AND cc.ContributionDate < DATEADD(hh, 24, @enddt)
				) Plg
	FROM Contributors(@startdt, @enddt, 0, 0, 0, 1, null) hh
	JOIN dbo.Families ff ON hh.FamilyId = ff.FamilyId
	JOIN dbo.Contribution c ON hh.PeopleId = c.PeopleId OR hh.SpouseId = c.PeopleId
	JOIN dbo.ContributionFund f ON c.FundId = f.FundId
	WHERE c.ContributionDate >= @startdt 
		AND c.ContributionDate < DATEADD(hh, 24, @enddt)
		AND ( (hh.PeopleId = ff.HeadOfHouseholdId AND hh.ContributionOptionsId = 2 AND hh.SpouseContributionOptionsId = 2)
				OR (hh.ContributionOptionsId <> 2)
			)
	GROUP BY hh.PeopleId, hh.spouseId, hh.Name, hh.SpouseName, f.FundId, f.FundName, hh.ContributionOptionsId, hh.SpouseContributionOptionsId
)

GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
