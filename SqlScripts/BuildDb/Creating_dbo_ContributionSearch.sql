CREATE FUNCTION [dbo].[ContributionSearch]
(
	@name NVARCHAR (100)
	,@comments NVARCHAR(100)
	,@bundletype INT
    ,@type INT
	,@status INT
	,@minamt MONEY
	,@maxamt MONEY 
	,@startdate DATETIME
	,@enddate DATETIME
	,@taxnontax VARCHAR(20)
	,@fundid INT
	,@campusid INT
	,@year INT
	,@includeunclosedbundles BIT
    ,@mobile BIT
	,@online INT
)
RETURNS 
@t TABLE (ContributionId INT)
AS
BEGIN
	DECLARE @pid INT = TRY_CONVERT(INT, @name)
	IF @pid > 0
		SET @name = NULL

	DECLARE @cid INT = TRY_CONVERT(INT, @comments)
	IF @cid > 0
		SET @comments = NULL

	INSERT @t (ContributionId)
		SELECT c.ContributionId
		FROM dbo.Contribution c
		JOIN dbo.BundleDetail d ON d.ContributionId = c.ContributionId
		JOIN dbo.BundleHeader h ON h.BundleHeaderId = d.BundleHeaderId
		LEFT JOIN dbo.People p ON p.PeopleId = c.PeopleId
		WHERE 1=1
		AND (ISNULL(@includeunclosedbundles, 0) = 0 OR h.BundleStatusId = 0)
		AND (ISNULL(@mobile, 0) = 0 OR c.SOURCE > 0)
		AND CASE ISNULL(@taxnontax, 'na')
			WHEN 'na' THEN 1
			WHEN 'taxded' THEN CASE WHEN c.ContributionTypeId NOT IN (8,9) THEN 1 ELSE 0 END
			WHEN 'nontaxded' THEN CASE WHEN c.ContributionTypeId = 9 THEN 1 ELSE 0 END 
			WHEN 'both' THEN CASE WHEN c.ContributionTypeId <> 8 THEN 1 ELSE 0 END 
			WHEN 'pledge' THEN CASE WHEN c.ContributionTypeId = 8 THEN 1 ELSE 0 END 
			END = 1
		AND CASE ISNULL(@online, 0)
			WHEN 0 THEN 1
			WHEN 1 THEN CASE WHEN h.BundleHeaderTypeId = 4 THEN 1 ELSE 0 END
            WHEN 0 THEN CASE WHEN h.BundleHeaderTypeId <> 4 THEN 1 ELSE 0 END
			END = 1
		AND CASE ISNULL(@status, 0)
			WHEN 0 THEN CASE WHEN c.ContributionStatusId = 0 AND c.ContributionTypeId NOT IN (6,7) THEN 1 ELSE 0 END
			WHEN 1 THEN CASE WHEN c.ContributionStatusId = 1 OR c.ContributionTypeId = 6 THEN 1 ELSE 0 END
			WHEN 2 THEN CASE WHEN c.ContributionStatusId = 2 OR c.ContributionTypeId = 7 THEN 1 ELSE 0 END
			END = 1
		AND (@minamt IS NULL OR c.ContributionAmount >= @minamt)
		AND (@maxamt IS NULL OR c.ContributionAmount <= @maxamt)
		AND (@pid IS NULL OR c.PeopleId = @pid)
		AND (@cid IS NULL OR c.ContributionId = @cid)
		AND (@name IS NULL OR p.NAME LIKE '%' + @name + '%')
		AND (@comments IS NULL OR c.ContributionDesc LIKE '%' + @comments + '%' OR c.CheckNo = @comments)
		AND (@type IS NULL OR c.ContributionTypeId = @type)
		AND (@campusid IS NULL OR c.CampusId = @campusid)
		AND (@bundletype IS NULL OR h.BundleHeaderTypeId = @bundletype)
		AND (@year IS NULL OR DATEPART(YEAR, c.ContributionDate) = @year)
		AND (@fundid IS NULL OR c.FundId = @fundid)
		AND (@startdate IS NULL OR c.ContributionDate >= @startdate)
		AND (@enddate IS NULL OR c.ContributionDate < DATEADD(DAY, 1, @enddate))
    
	/*
            if ((model.BundleType ?? 0) != 0)
                contributions = from c in contributions
                                where c.BundleDetails.First().BundleHeader.BundleHeaderTypeId == model.BundleType
                                select c;

            if (model.Year.HasValue && model.Year > 0)
                contributions = from c in contributions
                                where c.ContributionDate.Value.Year == model.Year
                                select c;

            if (model.FundId.HasValue && model.FundId > 0)
                contributions = from c in contributions
                                where c.FundId == model.FundId
                                select c;
	
*/
	
	RETURN 
END
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
