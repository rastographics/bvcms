CREATE FUNCTION [dbo].[ContributionSearch]
(	
	@Name VARCHAR(100)
	,@Comments VARCHAR(100)
	,@MinAmt MONEY
	,@MaxAmt MONEY
	,@StartDate DATETIME
	,@EndDate DATETIME
	,@CampusId INT
	,@FundId INT
	,@Online INT
	,@Status INT
	,@TaxNonTax VARCHAR(50)
	,@Year INT
	,@Type INT
	,@BundleType INT
	,@IncludeUnclosedBundles BIT
	,@Mobile BIT
	,@PeopleId INT
	,@ActiveTagFilter INT
	,@fundids VARCHAR(MAX)

)
RETURNS 
@t TABLE ( ContributionId INT PRIMARY KEY )
AS
BEGIN
	DECLARE @funds TABLE ( FundId INT )
	INSERT @funds (FundId) SELECT Value FROM dbo.SplitInts(@fundids)

	INSERT INTO @t (ContributionId)
	SELECT c.ContributionId
	FROM   dbo.Contribution c
	LEFT JOIN dbo.People p ON p.PeopleId = c.PeopleId
	WHERE CASE
			WHEN @Name IS NULL THEN 1
			WHEN TRY_PARSE(@Name AS INT) > 0 THEN IIF(c.PeopleId = TRY_PARSE(@Name AS INT), 1, 0)
			ELSE IIF(EXISTS(SELECT NULL FROM dbo.People p 
							WHERE p.PeopleId = c.PeopleId 
							AND p.NAME LIKE '%' + @Name + '%'), 1, 0)
			END = 1
	AND (@Comments IS NULL 
			OR c.ContributionDesc LIKE '%' + @Comments + '%'
			OR c.CheckNo = @Comments
			OR c.ContributionId = TRY_PARSE(@Comments AS INT))
	AND (@MinAmt IS NULL OR c.ContributionAmount >= @MinAmt)
	AND (@MaxAmt IS NULL OR c.ContributionAmount <= @MaxAmt)
	AND (@StartDate IS NULL OR c.ContributionDate >= @StartDate)
	AND (@EndDate IS NULL OR c.ContributionDate < DATEADD(DAY, 1, @EndDate))
	AND (ISNULL(@CampusId, 0) = 0 OR ISNULL(c.CampusId, p.CampusId) = @CampusId)
	AND (ISNULL(@FundId, 0) = 0 OR c.FundId = @FundId)
	AND CASE @Online
			WHEN 1 THEN IIF(EXISTS(SELECT NULL FROM dbo.BundleDetail d
									JOIN dbo.BundleHeader h ON h.BundleHeaderId = d.BundleHeaderId
									WHERE d.ContributionId = c.ContributionId
									AND h.BundleHeaderTypeId = 4), 1, 0)
			WHEN 0 THEN IIF(NOT EXISTS(SELECT NULL FROM dbo.BundleDetail d
									JOIN dbo.BundleHeader h ON h.BundleHeaderId = d.BundleHeaderId
									WHERE d.ContributionId = c.ContributionId
									AND h.BundleHeaderTypeId = 4), 1, 0)
			ELSE 1 END = 1
	AND CASE ISNULL(@Status, 0)
		   WHEN 0 THEN IIF(@PeopleId IS NULL AND c.ContributionStatusId = 0 AND c.ContributionTypeId NOT IN (6,7), 1, 0)
		   WHEN 2 THEN IIF(c.ContributionStatusId = 2 OR c.ContributionTypeId = 6, 1, 0)
		   WHEN 1 THEN IIF(c.ContributionStatusId = 1 OR c.ContributionTypeId = 7, 1, 0)
		   ELSE 1 END = 1
	AND CASE ISNULL(@TaxNonTax, 'TaxDed')
		   WHEN 'TaxDed' THEN IIF(c.ContributionTypeId NOT IN (9, 8), 1, 0)
		   WHEN 'NonTaxDed' THEN IIF(c.ContributionTypeId = 9, 1, 0)
		   WHEN 'Both' THEN IIF(c.ContributionTypeId <> 8, 1, 0)
		   WHEN 'Pledge' THEN IIF(c.ContributionTypeId = 8, 1, 0)
		   ELSE 1 END = 1
	AND (ISNULL(@Year, 0) = 0 OR DATEPART(YEAR, c.ContributionDate) = @Year)
	AND (ISNULL(@Type, 0) = 0 OR c.ContributionTypeId = @Type)
	AND CASE WHEN @BundleType = 9999
				THEN IIF(NOT EXISTS(SELECT NULL FROM dbo.BundleDetail WHERE ContributionId = c.ContributionId), 1, 0)
			WHEN ISNULL(@BundleType, 0) <> 0 
				THEN IIF(EXISTS(SELECT NULL FROM dbo.BundleDetail d 
								JOIN dbo.BundleHeader h ON h.BundleHeaderId = d.BundleHeaderId 
								WHERE ContributionId = c.ContributionId
								AND h.BundleHeaderTypeId = @BundleType), 1, 0)
		ELSE 1 END = 1
	AND CASE ISNULL(@IncludeUnclosedBundles, 0)
		WHEN 0 THEN IIF(EXISTS(SELECT NULL FROM dbo.BundleDetail d
					JOIN dbo.BundleHeader h ON h.BundleHeaderId = d.BundleHeaderId
					WHERE c.ContributionId = d.ContributionId AND h.BundleStatusId = 0), 1, 0)
		ELSE 1 END = 1
	AND (ISNULL(@Mobile, 0) <> 1 OR c.Source > 0)
	AND (@PeopleId IS NULL OR c.PeopleId = @PeopleId)
	AND (@ActiveTagFilter IS NULL OR EXISTS(SELECT NULL FROM dbo.TagPerson tp WHERE tp.PeopleId = c.PeopleId AND tp.Id = @ActiveTagFilter))
	AND (@fundids IS NULL OR EXISTS(SELECT NULL FROM @funds f WHERE f.FundId = c.FundId))
	RETURN

	
	RETURN 
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
