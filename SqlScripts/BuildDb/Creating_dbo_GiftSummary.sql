CREATE FUNCTION [dbo].[GiftSummary]
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
		f.FundName
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
	GROUP BY f.FundName
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
