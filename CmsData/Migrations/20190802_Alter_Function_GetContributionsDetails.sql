ALTER FUNCTION [dbo].[GetContributionsDetails]
(
	@fd DATETIME, 
	@td DATETIME,
	@campusid INT,
	@pledges BIT,
	@nontaxded INT,
	@includeUnclosed BIT,
	@tagid INT,
	@fundids VARCHAR(MAX)
)
RETURNS TABLE 
AS
RETURN 
(
	WITH details as (SELECT 
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
		ELSE p.Name2 
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
from dbo.ContributionSearch(null, null, null, null, @fd, @td, @campusid, null, null, -1, 
	case @nontaxded WHEN 1 then 'NonTaxDed' 
	when 0 then 'TaxDed' 
	else IIF(@pledges = 1, '', 'Both') END,
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
WHERE CASE WHEN @pledges = 1 THEN 1
	  WHEN c.ContributionTypeId = 8 THEN 0  -- NO PLEDGES
	  ELSE 1 END = 1
	  )
	,contributions AS ( 
		SELECT 
			CreditGiverId, 
			CreditGiverId2,
			HeadName, 
			SpouseName,
			FundId,
			FundName,
			COUNT(*) AS [Count], 
			SUM(Amount) AS Amount, 
			SUM(PledgeAmount) PledgeAmount
		FROM details
		GROUP BY CreditGiverId, CreditGiverId2, HeadName, SpouseName, FundId, FundName
	)
	SELECT 
		c.CreditGiverId, 
		c.CreditGiverId2,
		c.HeadName,
		c.SpouseName,
		c.[Count],
		c.Amount,
		c.PledgeAmount,
		c.FundId,
		c.FundName,
		ISNULL(o.OrganizationName, '') MainFellowship, 
		ms.[Description] MemberStatus, 
		p.JoinDate, 
		p.SpouseId, 
		op.[Description] [Option],
		p.PrimaryAddress Addr,
		p.PrimaryAddress2 Addr2,
		p.PrimaryCity City,
		p.PrimaryState ST,
		p.PrimaryZip Zip
	FROM contributions c
	LEFT JOIN dbo.People p ON p.PeopleId = c.CreditGiverId
	LEFT JOIN lookup.MemberStatus ms ON p.MemberStatusId = ms.Id
	LEFT JOIN lookup.EnvelopeOption op ON op.Id = p.ContributionOptionsId
	LEFT OUTER JOIN dbo.Organizations o ON o.OrganizationId = p.BibleFellowshipClassId
)
GO