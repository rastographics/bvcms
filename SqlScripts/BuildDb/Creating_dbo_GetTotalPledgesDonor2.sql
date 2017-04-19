CREATE FUNCTION [dbo].[GetTotalPledgesDonor2]
(
	@fd DATETIME, 
	@td DATETIME,
	@campusid INT,
	@pledgefund INT
)
RETURNS TABLE
AS
RETURN 
(
	WITH contributions AS ( 
		SELECT 
			CreditGiverId, 
			CreditGiverId2,
			HeadName, 
			SpouseName, 
			COUNT(*) AS [Count], 
			SUM(PledgeAmount) AS PledgeAmount,
			SUM(Amount) AS Amount
		FROM dbo.Contributions2(@fd, @td, @campusid, NULL, NULL, 1)
		WHERE ISNULL(@pledgefund, 0) = 0 OR FundId = @pledgefund
		GROUP BY CreditGiverId, CreditGiverId2, HeadName, SpouseName
	)
	SELECT 
		c.CreditGiverId, 
		c.CreditGiverId2,
		c.HeadName,
		c.SpouseName,
		c.[Count],
		c.PledgeAmount,
		c.Amount,
		Balance = IIF(c.PledgeAmount > 0, c.PledgeAmount - ISNULL(c.Amount, 0), 0),
		MainFellowship = ISNULL(o.OrganizationName, ''), 
		MemberStatus = ms.[Description], 
		p.JoinDate, 
		p.SpouseId, 
		[Option] = op.[Description],
		Addr = p.PrimaryAddress,
		Addr2 = p.PrimaryAddress2,
		City = p.PrimaryCity,
		ST = p.PrimaryState,
		Zip = p.PrimaryZip
	FROM contributions c
	JOIN dbo.People p ON p.PeopleId = c.CreditGiverId
	JOIN lookup.MemberStatus ms ON p.MemberStatusId = ms.Id
	LEFT JOIN lookup.EnvelopeOption op ON op.Id = p.ContributionOptionsId
	LEFT OUTER JOIN dbo.Organizations o ON o.OrganizationId = p.BibleFellowshipClassId
)

GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
