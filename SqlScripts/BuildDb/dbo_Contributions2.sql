
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
			AND sp.ContributionOptionsId = 2
			AND p.ContributionOptionsId = 2
		THEN sp.PeopleId 
		ELSE c.PeopleId 
	END AS CreditGiverId,
	
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
	AND ((CASE WHEN c.ContributionTypeId = 9 THEN 1 ELSE ISNULL(f.NonTaxDeductible, 0) END) = @nontaxded OR @nontaxded IS NULL)
    AND c.ContributionStatusId = 0 -- recorded
    --AND ((CASE WHEN c.ContributionTypeId = 8 THEN 1 ELSE 0 END) = @pledges OR @pledges IS NULL)
    AND c.ContributionDate >= @fd AND c.ContributionDate < DATEADD(hh, 24, @td)
	AND (h.BundleStatusId = 0 OR @includeUnclosed = 1)
    AND (@campusid = 0 OR p.CampusId = @campusid)
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
