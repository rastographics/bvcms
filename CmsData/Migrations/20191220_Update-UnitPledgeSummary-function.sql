
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
			MIN(c.ContributionDate) PledgeDate,
			SUM(c.ContributionAmount) Total
		FROM dbo.Contribution c
		JOIN dbo.ContributionFund f ON f.FundId = c.FundId
		JOIN dbo.People p ON p.PeopleId = c.PeopleId
		WHERE c.ContributionStatusId <> 1 -- not reversed
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
			SUM(c.ContributionAmount) Total
		FROM dbo.Contribution c
		JOIN dbo.ContributionFund f ON f.FundId = c.FundId
		JOIN dbo.People p ON p.PeopleId = c.PeopleId
		WHERE c.ContributionStatusId = 0 -- Recorded
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
        b.PledgeDate,
		g.Total Given,
		b.Total Pledged
	FROM pledgebals b
	LEFT JOIN gifttotals g ON g.FundId = b.FundId
	WHERE (EXISTS(SELECT NULL FROM dbo.Setting WHERE Id = 'ShowPledgeIfMet' AND Setting = 'true')
			OR ISNULL(b.Total, 0) > ISNULL(g.Total, 0))
)
GO