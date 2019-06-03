IF NOT EXISTS (
	SELECT type_desc, type
	FROM SYS.PROCEDURES WITH(NOLOCK)
	WHERE NAME = 'GreatPlainsIncomeExport'
		AND type = 'P')
	BEGIN
		DECLARE @Query[nvarchar](max); 
		SET @Query = N'CREATE PROC [dbo].[GreatPlainsIncomeExport]
		(
			@fd DATETIME, 
			@td DATETIME,
			@campusid INT,
			@nontaxded BIT,
			@includeUnclosed BIT
		)
		AS
		BEGIN
		;WITH totals AS (
			SELECT 
				c2.[Date],
				FundName,
				BundleHeaderTypeId,
				c2.FundId,
				SUM(Amount) AS Amount,
				CASE WHEN BundleHeaderTypeId = 4 THEN 1 ELSE 0 END AS [OnLine]
			FROM dbo.Contributions2(@fd, @td, @campusid, NULL, @nontaxded, @includeUnclosed) c2
			JOIN dbo.BundleHeader h ON c2.BundleHeaderId = h.BundleHeaderId
			GROUP BY c2.FundId, FundName, c2.[Date],
				CASE WHEN BundleHeaderTypeId = 4 THEN 1 ELSE 0 END,
				BundleHeaderTypeId
		)
			SELECT 
				tt.[Date],
				tt.FundName,
				f.FundIncomeAccount,
				bht.DESCRIPTION BundleType,
				tt.Amount
			FROM totals tt
			JOIN lookup.BundleHeaderTypes bht ON tt.BundleHeaderTypeId = bht.Id
			JOIN dbo.ContributionFund f ON f.FundId = tt.FundId
			ORDER BY f.FundId, bht.Description
		END'
		EXEC sys.sp_executesql @Query
	END
GO