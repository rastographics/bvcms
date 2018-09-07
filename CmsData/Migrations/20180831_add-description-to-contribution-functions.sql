ALTER FUNCTION [dbo].[GiftsInKind]
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
		[Description] = c.ContributionDesc,
		f.FundDescription
	FROM dbo.Contribution c
	JOIN dbo.ContributionFund f ON f.FundId = c.FundId
	JOIN dbo.People p ON p.PeopleId = c.PeopleId
	WHERE 1=1
	AND c.ContributionStatusId = 0 -- Recorded
	AND c.contributionTypeId IN (10, 20) -- GiftinKind or Stock
	AND c.ContributionDate >= @fromDate
	AND CONVERT(DATE, c.ContributionDate) <= @toDate
	AND (c.PeopleId = @pid OR (@joint = 1 AND c.PeopleId = @spid))
	AND (@fundids IS NULL OR EXISTS(SELECT NULL FROM dbo.SplitInts(@fundids) WHERE Value = c.FundId))
)
GO

ALTER FUNCTION [dbo].[NonTaxContributions]
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
		[Description] = c.ContributionDesc,
		f.FundDescription
	FROM dbo.Contribution c
	JOIN dbo.ContributionFund f ON f.FundId = c.FundId
	JOIN dbo.People p ON p.PeopleId = c.PeopleId
	WHERE 1=1
	AND c.ContributionStatusId = 0 -- Recorded
	AND c.ContributionTypeId NOT IN (6, 7) -- not returned or reversed
	AND c.contributionTypeId NOT IN (10, 20) -- not stock or giftinkind
	AND c.ContributionTypeId <> 8 -- not pledge
	AND (f.NonTaxDeductible = 1 OR c.ContributionTypeId = 9)
	AND c.ContributionDate >= @fromDate
	AND CONVERT(DATE, c.ContributionDate) <= @toDate
	AND (c.PeopleId = @pid OR (@joint = 1 AND c.PeopleId = @spid))
	AND (@fundids IS NULL OR EXISTS(SELECT NULL FROM dbo.SplitInts(@fundids) WHERE Value = c.FundId))
)
GO

ALTER FUNCTION [dbo].[NormalContributions]
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
		f.FundName as FundName,
		c.CheckNo,
		p.[Name],
		[Description] = c.ContributionDesc,
		f.FundDescription
	FROM dbo.Contribution c
	JOIN dbo.ContributionFund f ON f.FundId = c.FundId
	JOIN dbo.People p ON p.PeopleId = c.PeopleId
	WHERE 1=1
	AND c.ContributionStatusId = 0 -- Recorded
	AND c.ContributionTypeId NOT IN (6, 7) -- not returned or reversed
	AND c.contributionTypeId NOT IN (10, 20) -- not stock or giftinkind
	AND c.ContributionTypeId NOT IN (8, 9) -- not pledge or nontaxded
	AND ISNULL(f.NonTaxDeductible, 0) = 0
	AND c.ContributionDate >= @fromDate
	AND CONVERT(DATE, c.ContributionDate) <= @toDate
	AND (c.PeopleId = @pid OR (@joint = 1 AND c.PeopleId = @spid))
	AND (@fundids IS NULL OR EXISTS(SELECT NULL FROM dbo.SplitInts(@fundids) WHERE Value = c.FundId))
)
GO

ALTER FUNCTION [dbo].[UnitPledgeSummary]
(
	@pid INT, 
	@spid INT, 
	@joint BIT, 
	@toDate DATE,
	@fundids VARCHAR(MAX)
)
RETURNS TABLE 
AS
RETURN 
(
	WITH pledgebals AS (
		SELECT
			c.FundId,
			f.FundName,
			f.FundDescription,
			Total = SUM(c.ContributionAmount)
		FROM dbo.Contribution c
		JOIN dbo.ContributionFund f ON f.FundId = c.FundId
		JOIN dbo.People p ON p.PeopleId = c.PeopleId
		WHERE 1=1
		AND c.ContributionStatusId <> 1 -- not reversed
		AND c.contributionTypeId = 8 -- Pledge
		AND f.FundStatusId = 1 -- Active
		AND f.FundPledgeFlag = 1
		AND CONVERT(DATE, c.ContributionDate) <= @toDate
		AND (c.PeopleId = @pid OR (@joint = 1 AND c.PeopleId = @spid))
		AND (@fundids IS NULL OR EXISTS(SELECT NULL FROM dbo.SplitInts(@fundids) WHERE Value = c.FundId))
		GROUP BY c.FundId, f.FundName, f.FundDescription
	),
	gifttotals AS (
		SELECT
			c.FundId,
			f.FundName,
			f.FundDescription,
			Total = SUM(c.ContributionAmount)
		FROM dbo.Contribution c
		JOIN dbo.ContributionFund f ON f.FundId = c.FundId
		JOIN dbo.People p ON p.PeopleId = c.PeopleId
		WHERE 1=1
		AND c.ContributionStatusId = 0 -- Recorded
		AND c.ContributionTypeId NOT IN (6, 7) -- not returned or reversed
		AND c.contributionTypeId NOT IN (8, 10) -- not pledge or giftinkind
		AND f.FundStatusId = 1 -- Active
		AND f.FundPledgeFlag = 1
		AND CONVERT(DATE, c.ContributionDate) <= @toDate
		AND (c.PeopleId = @pid OR (@joint = 1 AND c.PeopleId = @spid))
		AND (@fundids IS NULL OR EXISTS(SELECT NULL FROM dbo.SplitInts(@fundids) WHERE Value = c.FundId))
		GROUP BY c.FundId, f.FundName, f.FundDescription
	)
	SELECT 
		b.FundName,
		b.FundDescription,
		Given = g.Total,
		Pledged = b.Total
	FROM pledgebals b
	LEFT JOIN gifttotals g ON g.FundId = b.FundId
	WHERE (EXISTS(SELECT NULL FROM dbo.Setting WHERE Id = 'ShowPledgeIfMet' AND Setting = 'true')
			OR ISNULL(b.Total, 0) > ISNULL(g.Total, 0))
)
GO

ALTER FUNCTION [dbo].[GiftSummary]
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
		Total = SUM(c.ContributionAmount),
		f.FundName,
		f.FundDescription
	FROM dbo.Contribution c
	JOIN dbo.ContributionFund f ON f.FundId = c.FundId
	WHERE 1=1
	AND c.ContributionStatusId = 0 -- Recorded
	-- no returned, reversed, pledge, nontax, giftinkind, stock
	AND c.ContributionTypeId NOT IN (6, 7, 8, 9, 10, 20) 
	AND ISNULL(f.NonTaxDeductible, 0) = 0
	AND c.ContributionDate >= @fromDate
	AND CONVERT(DATE, c.ContributionDate) <= @toDate
	AND (c.PeopleId = @pid OR (@joint = 1 AND c.PeopleId = @spid))
	AND (@fundids IS NULL OR EXISTS(SELECT NULL FROM dbo.SplitInts(@fundids) WHERE Value = c.FundId))
	GROUP BY f.FundName, f.FundDescription
)
GO

