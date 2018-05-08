CREATE FUNCTION [dbo].[GetTotalContributionsDonorFund]
(
	@fd DATETIME, 
	@td DATETIME,
	@campusid INT,
	@nontaxded BIT,
	@includeUnclosed BIT,
	@tagid INT,
	@fundids VARCHAR(MAX)
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT tt.*, 
	(SELECT TOP 1 OrganizationName FROM dbo.Organizations o WHERE o.OrganizationId = p.BibleFellowshipClassId) MainFellowship,
	(SELECT TOP 1 Description FROM lookup.MemberStatus ms WHERE p.MemberStatusId = ms.Id) MemberStatus,
	p.JoinDate
	FROM
	(
	SELECT 
		CreditGiverId, 
		(HeadName + CASE WHEN SpouseId <> CreditGiverId THEN '*' ELSE '' END) HeadName, 
		(SpouseName + CASE WHEN SpouseId = CreditGiverId THEN '*' ELSE '' END) SpouseName, 
		COUNT(*) AS [Count], 
		SUM(Amount) AS Amount, 
		SUM(PledgeAmount) AS PledgeAmount, 
		c2.FundId, 
		FundName
	FROM dbo.GetContributionsDetails(@fd, @td, @campusid, NULL, @nontaxded, @includeUnclosed, @tagid, @fundids) c2
	GROUP BY CreditGiverId, HeadName, SpouseId, SpouseName, SpouseId, c2.FundId, FundName
	) tt 
	LEFT JOIN dbo.People p ON p.PeopleId = tt.CreditGiverId
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
