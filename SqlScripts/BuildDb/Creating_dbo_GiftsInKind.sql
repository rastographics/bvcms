CREATE FUNCTION [dbo].[GiftsInKind]
(
	@pid INT, 
	@spid INT, 
	@joint BIT, 
	@fromDate DATE,
	@toDate DATE,
	@fundids VARCHAR(MAX)
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT 
		c.ContributionId,
		c.ContributionAmount,
		c.ContributionDate,
		f.FundName,
		c.CheckNo,
		p.[Name],
		[Description] = c.ContributionDesc
	FROM dbo.Contribution c
	JOIN dbo.ContributionFund f ON f.FundId = c.FundId
	JOIN dbo.People p ON p.PeopleId = c.PeopleId
	WHERE 1=1
	AND c.ContributionStatusId = 0 -- Recorded
	AND c.contributionTypeId IN (10, 20) -- GiftinKind or Stock
	AND c.ContributionDate >= @fromDate
	AND CONVERT(DATE, c.ContributionDate) <= @toDate
	AND (c.PeopleId = @pid OR (@joint = 1 AND c.PeopleId = @spid))
	AND (@fundids IS NULL OR c.FundId IN (SELECT Value FROM dbo.SplitInts(@fundids)))
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
