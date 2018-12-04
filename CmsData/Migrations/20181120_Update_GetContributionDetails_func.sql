ALTER FUNCTION [dbo].[GetContributionsDetails]
(
	@fd DATETIME, 
	@td DATETIME,
	@campusid INT,
	@pledges BIT,
	@nontaxded BIT,
	@includeUnclosed BIT,
	@tagid INT,
	@fundids VARCHAR(MAX)
)
RETURNS TABLE 
AS
RETURN 
(
SELECT 
	p.FamilyId,
	p.PeopleId,
    c.ContributionDate AS Date,
    
    CASE WHEN fa.HeadOfHouseholdId = sp.PeopleId
			AND ISNULL(sp.ContributionOptionsId, CASE WHEN sp.MaritalStatusId = 20 THEN 2 ELSE 1 END) = 2
			AND ISNULL(p.ContributionOptionsId, CASE WHEN p.MaritalStatusId = 20 THEN 2 ELSE 1 END) = 2
		THEN sp.PeopleId 
		ELSE c.PeopleId 
	END AS CreditGiverId,

    CASE WHEN ISNULL(sp.ContributionOptionsId, CASE WHEN sp.MaritalStatusId = 20 THEN 2 ELSE 1 END) = 1
			OR ISNULL(p.ContributionOptionsId, CASE WHEN p.MaritalStatusId = 20 THEN 2 ELSE 1 END) = 1
		THEN NULL
		WHEN fa.HeadOfHouseholdId = sp.PeopleId
		THEN c.PeopleId
		ELSE sp.PeopleId
	END AS CreditGiverId2,

    CASE WHEN fa.HeadOfHouseholdId = sp.PeopleId
		THEN p.PeopleId 
		ELSE sp.PeopleId 
	END AS SpouseId,
	
    CASE WHEN fa.HeadOfHouseholdId = sp.PeopleId
		THEN sp.Name2 
		ELSE ISNULL(p.Name2, CONCAT(p.LastName, ', ', p.FirstName)) 
	END AS HeadName,
	
    CASE WHEN fa.HeadOfHouseholdId = sp.PeopleId
		THEN p.Name2 
		ELSE sp.Name2 
	END AS SpouseName,
	
	CASE WHEN ContributionTypeId <> 8
		THEN ContributionAmount
		ELSE 0
	END AS Amount,
	
	CASE WHEN ContributionTypeId = 8
		THEN ContributionAmount
		ELSE 0
	END AS PledgeAmount,
	
    h.BundleHeaderId,
	c.ContributionDesc,
	c.CheckNo,
    c.FundId,
    f.FundName,
    CASE WHEN f.FundPledgeFlag = 1 AND f.FundStatusId = 1
		THEN 1
		ELSE 0
	END AS OpenPledgeFund,
    bht.Description AS BundleType,
    bst.Description AS BundleStatus,
    c.ContributionId
from dbo.ContributionSearch(null, null, null, null, @fd, @td, @campusid, null, null, 0, 
	case when @nontaxded = 1 then 'NonTaxDed' when @nontaxded = 0 then 'TaxDed' else 'Both' end, 
	null, null, null, @includeUnclosed, null, null, @tagid, @fundids) cs
join dbo.Contribution c on c.ContributionId = cs.ContributionId
	JOIN dbo.ContributionFund f ON c.FundId = f.FundId
	LEFT JOIN dbo.BundleDetail d ON c.ContributionId = d.ContributionId
	LEFT JOIN dbo.BundleHeader h ON d.BundleHeaderId = h.BundleHeaderId
	LEFT JOIN lookup.BundleHeaderTypes bht ON h.BundleHeaderTypeId = bht.Id
	LEFT JOIN lookup.BundleStatusTypes bst ON h.BundleStatusId = bst.Id
	LEFT JOIN dbo.People p ON c.PeopleId = p.PeopleId
	LEFT JOIN dbo.Families fa ON p.FamilyId = fa.FamilyId
	LEFT JOIN dbo.People sp ON sp.PeopleId = p.SpouseId
WHERE c.ContributionTypeId <> 8 -- NO PLEDGES
)
GO
