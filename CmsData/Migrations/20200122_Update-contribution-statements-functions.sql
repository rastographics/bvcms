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
		f.FundName,
		c.CheckNo,
		p.[Name],
		c.ContributionDesc [Description],
		f.FundDescription
	FROM dbo.Contribution c
	JOIN lookup.ContributionType ct ON ct.Id = c.ContributionTypeId
	JOIN dbo.ContributionFund f ON f.FundId = c.FundId
	JOIN dbo.People p ON p.PeopleId = c.PeopleId
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
		f.FundName,
		c.CheckNo,
		p.[Name],
		c.ContributionDesc [Description],
		f.FundDescription
	FROM dbo.Contribution c
	JOIN lookup.ContributionType ct ON ct.Id = c.ContributionTypeId
	JOIN dbo.ContributionFund f ON f.FundId = c.FundId
	JOIN dbo.People p ON p.PeopleId = c.PeopleId
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
		f.FundName,
		c.CheckNo,
		p.[Name],
		c.ContributionDesc [Description],
		f.FundDescription
	FROM dbo.Contribution c
	JOIN lookup.ContributionType ct ON ct.Id = c.ContributionTypeId
	JOIN dbo.ContributionFund f ON f.FundId = c.FundId
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