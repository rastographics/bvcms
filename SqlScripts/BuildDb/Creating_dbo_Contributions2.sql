CREATE FUNCTION [dbo].[Contributions2]
(
	@fd DATETIME, 
	@td DATETIME,
	@campusid INT,
	@pledges BIT,
	@nontaxded BIT,
	@includeUnclosed BIT
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
    c.QBSyncID
FROM dbo.Contribution c
	JOIN dbo.ContributionFund f ON c.FundId = f.FundId
	LEFT JOIN dbo.BundleDetail d ON c.ContributionId = d.ContributionId
	LEFT JOIN dbo.BundleHeader h ON d.BundleHeaderId = h.BundleHeaderId
	LEFT JOIN lookup.BundleHeaderTypes bht ON h.BundleHeaderTypeId = bht.Id
	LEFT JOIN lookup.BundleStatusTypes bst ON h.BundleStatusId = bst.Id
	JOIN dbo.People p ON c.PeopleId = p.PeopleId
	JOIN dbo.Families fa ON p.FamilyId = fa.FamilyId
	LEFT JOIN dbo.People sp ON sp.PeopleId = p.SpouseId
WHERE 1 = 1
	AND c.ContributionTypeId NOT IN (6,7) -- no reversed or returned
	-- @nontaxded = 1 = only nontax, @nontaxded = 0 = only taxded, @nontaxded = null = either
	AND ((CASE WHEN c.ContributionTypeId = 9 THEN 1 ELSE ISNULL(f.NonTaxDeductible, 0) END) = @nontaxded OR @nontaxded IS NULL)
    AND c.ContributionStatusId = 0 -- recorded
	AND (c.ContributionTypeId <> 8 OR ISNULL(@pledges, 1) = 1)
    AND c.ContributionDate >= @fd AND c.ContributionDate < DATEADD(hh, 24, ISNULL(@td, CONVERT(DATE,GETDATE())))
	AND (ISNULL(h.BundleStatusId, 0) = 0 OR @includeUnclosed = 1)
    AND (@campusid = 0 OR c.CampusId = @campusid) -- campusid = 0 = all
)

GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
