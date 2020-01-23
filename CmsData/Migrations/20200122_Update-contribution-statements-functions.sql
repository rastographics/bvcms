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
	WITH funds AS (
        SELECT 
			f.FundId,
			f.FundName,
			f.FundDescription
        FROM dbo.ContributionFund f
        WHERE (@fundids IS NULL OR EXISTS(SELECT NULL FROM dbo.SplitInts(@fundids) WHERE Value = f.FundId))
		AND f.FundStatusId = 1 -- Active
		AND f.FundPledgeFlag = 1
    ),
    peopleids AS (
        SELECT p.PeopleId 
        FROM dbo.People p
		WHERE (p.PeopleId = @pid OR (@joint = 1 AND p.PeopleId = @spid))
    ),
    pledgebals AS (
		SELECT
			c.FundId,
			f.FundName,
			f.FundDescription,
			MIN(c.ContributionDate) PledgeDate,
			SUM(c.ContributionAmount) Total
		FROM dbo.Contribution c
		JOIN funds f ON f.FundId = c.FundId
		JOIN peopleids p ON p.PeopleId = c.PeopleId
		WHERE c.ContributionStatusId <> 1 -- not reversed
		AND c.contributionTypeId = 8 -- Pledge
		AND c.ContributionDate <= @toDate
		GROUP BY c.FundId, f.FundName, f.FundDescription
	),
    currentYear AS (
		SELECT
			c.FundId,
			SUM(c.ContributionAmount) Total
		FROM dbo.Contribution c
		JOIN funds f ON f.FundId = c.FundId
		JOIN peopleids p ON p.PeopleId = c.PeopleId
		WHERE c.ContributionStatusId <> 1 -- not reversed
		AND c.contributionTypeId = 8 -- Pledge
		AND YEAR(c.ContributionDate) = YEAR(@toDate)
		GROUP BY c.FundId
	),
	gifttotals AS (
		SELECT
			c.FundId,
			SUM(c.ContributionAmount) Total
		FROM dbo.Contribution c
		JOIN funds f ON f.FundId = c.FundId
		JOIN peopleids p ON p.PeopleId = c.PeopleId
		WHERE c.ContributionStatusId = 0 -- Recorded
		AND c.ContributionTypeId NOT IN (6, 7) -- not returned or reversed
		AND c.contributionTypeId NOT IN (8, 10) -- not pledge or giftinkind
		AND c.ContributionDate <= @toDate
		GROUP BY c.FundId
	)
	SELECT 
		b.FundName,
		b.FundDescription,
        b.PledgeDate,
        (SELECT MAX(t)
        FROM (VALUES (0),(ISNULL(g.Total, 0) - ISNULL(curryear.Total, 0))) AS totals(t)) [PriorYearsTotal],
        ISNULL(curryear.Total, 0) [CurrentYearTotal],
		ISNULL(g.Total, 0) Given,
		b.Total Pledged
	FROM pledgebals b
	LEFT JOIN gifttotals g ON g.FundId = b.FundId
    LEFT JOIN currentYear curryear ON curryear.FundId = b.FundId
	WHERE (EXISTS(SELECT NULL FROM dbo.Setting WHERE Id = 'ShowPledgeIfMet' AND Setting = 'true')
			OR ISNULL(b.Total, 0) > ISNULL(g.Total, 0))
)
GO

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
		c.ContributionTypeId,
		ct.Description ContributionType,
		bt.Description BundleType,
		f.FundName,
		c.CheckNo,
		p.[Name],
		c.ContributionDesc [Description],
		f.FundDescription
	FROM dbo.Contribution c
	JOIN lookup.ContributionType ct ON ct.Id = c.ContributionTypeId
	JOIN dbo.ContributionFund f ON f.FundId = c.FundId
	JOIN dbo.People p ON p.PeopleId = c.PeopleId
	JOIN dbo.BundleDetail bd on bd.ContributionId = c.ContributionId
	JOIN dbo.BundleHeader bh on bh.BundleHeaderId = bd.BundleHeaderId
	JOIN lookup.BundleHeaderTypes bt on bt.Id = bh.BundleHeaderTypeId
	WHERE c.ContributionStatusId = 0 -- Recorded
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
        c.ContributionTypeId,
		ct.Description ContributionType,
		bt.Description BundleType,
		f.FundName,
		c.CheckNo,
		p.[Name],
		c.ContributionDesc [Description],
		f.FundDescription
	FROM dbo.Contribution c
	JOIN lookup.ContributionType ct ON ct.Id = c.ContributionTypeId
	JOIN dbo.ContributionFund f ON f.FundId = c.FundId
	JOIN dbo.People p ON p.PeopleId = c.PeopleId
	JOIN dbo.BundleDetail bd on bd.ContributionId = c.ContributionId
	JOIN dbo.BundleHeader bh on bh.BundleHeaderId = bd.BundleHeaderId
	JOIN lookup.BundleHeaderTypes bt on bt.Id = bh.BundleHeaderTypeId
	WHERE c.ContributionStatusId = 0 -- Recorded
	AND c.ContributionTypeId NOT IN (6, 7, -- not returned or reversed
	                               10, 20, -- not stock or giftinkind
	                                    8) -- not pledge
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
        c.ContributionTypeId,
		ct.Description ContributionType,
        bt.Description BundleType,
		f.FundName,
		c.CheckNo,
		p.[Name],
		c.ContributionDesc [Description],
		f.FundDescription
	FROM dbo.Contribution c
	JOIN lookup.ContributionType ct ON ct.Id = c.ContributionTypeId
	JOIN dbo.ContributionFund f ON f.FundId = c.FundId
	JOIN dbo.BundleDetail bd on bd.ContributionId = c.ContributionId
	JOIN dbo.BundleHeader bh on bh.BundleHeaderId = bd.BundleHeaderId
	JOIN lookup.BundleHeaderTypes bt on bt.Id = bh.BundleHeaderTypeId
	JOIN dbo.People p ON p.PeopleId = c.PeopleId
	WHERE c.ContributionStatusId = 0 -- Recorded
	AND c.ContributionTypeId NOT IN (6, 7, -- not returned or reversed
								   10, 20, -- not stock or giftinkind
									 8, 9) -- not pledge or nontaxded
	AND ISNULL(f.NonTaxDeductible, 0) = 0
	AND c.ContributionDate >= @fromDate
	AND CONVERT(DATE, c.ContributionDate) <= @toDate
	AND (c.PeopleId = @pid OR (@joint = 1 AND c.PeopleId = @spid))
	AND (@fundids IS NULL OR EXISTS(SELECT NULL FROM dbo.SplitInts(@fundids) WHERE Value = c.FundId))
)
GO