
CREATE FUNCTION [dbo].[GetTotalContributionsDonor2]
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
	SELECT tt.*, ISNULL(o.OrganizationName, '') MainFellowship, ms.Description MemberStatus FROM
	(
	SELECT 
		CreditGiverId, 
		HeadName, 
		SpouseName, 
		COUNT(*) AS [Count], 
		SUM(Amount) AS Amount, 
		SUM(PledgeAmount) AS PledgeAmount
	FROM dbo.Contributions2(@fd, @td, @campusid, NULL, @nontaxded, @includeUnclosed)
	GROUP BY CreditGiverId, HeadName, SpouseName
	) tt 
	JOIN dbo.People p ON p.PeopleId = tt.CreditGiverId
	JOIN lookup.MemberStatus ms ON p.MemberStatusId = ms.Id
	LEFT OUTER JOIN dbo.Organizations o ON o.OrganizationId = p.BibleFellowshipClassId
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
