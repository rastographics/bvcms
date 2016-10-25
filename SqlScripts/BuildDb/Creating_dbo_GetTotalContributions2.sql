
CREATE FUNCTION [dbo].[GetTotalContributions2]
(
	@fd DATETIME, 
	@td DATETIME,
	@campusid INT,
	@nontaxded BIT,
	@includeUnclosed BIT
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT 
		CreditGiverId, 
		HeadName,
		SpouseName,
		[Count],
		Amount,
		PledgeAmount,
		FundId,
		FundName,
		[OnLine],
		QBSynced,
		o.OrganizationName MainFellowship, 
		ms.Description MemberStatus, 
		bht.Description BundleType 
	FROM
	(
	SELECT 
		CreditGiverId, 
		HeadName, 
		SpouseName, 
		COUNT(*) AS [Count], 
		SUM(Amount) AS Amount, 
		SUM(PledgeAmount) AS PledgeAmount, 
		c2.FundId, 
		FundName,
		BundleHeaderTypeId,
		CASE WHEN BundleHeaderTypeId = 4 THEN 1 ELSE 0 END AS [OnLine],
		CASE WHEN QBSyncID IS NULL THEN 0 ELSE 1 END QBSynced
	FROM dbo.Contributions2(@fd, @td, @campusid, NULL, @nontaxded, @includeUnclosed) c2
	JOIN dbo.BundleHeader h ON c2.BundleHeaderId = h.BundleHeaderId
	GROUP BY CreditGiverId, HeadName, SpouseName, c2.FundId, FundName, 
		CASE WHEN BundleHeaderTypeId = 4 THEN 1 ELSE 0 END,
		CASE WHEN QBSyncID IS NULL THEN 0 ELSE 1 END,
		BundleHeaderTypeId
	) tt 
	JOIN dbo.People p ON p.PeopleId = tt.CreditGiverId
	JOIN lookup.MemberStatus ms ON p.MemberStatusId = ms.Id
	JOIN lookup.BundleHeaderTypes bht ON tt.BundleHeaderTypeId = bht.Id
	LEFT OUTER JOIN dbo.Organizations o ON o.OrganizationId = p.BibleFellowshipClassId
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
