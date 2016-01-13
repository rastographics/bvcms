
CREATE PROC [dbo].[LedgerIncomeExport]
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
		COUNT(*) AS [Count], 
		SUM(Amount) AS Amount, 
		c2.FundId, 
		FundName,
		BundleHeaderTypeId,
		CASE WHEN BundleHeaderTypeId = 4 THEN 1 ELSE 0 END AS [OnLine]
	FROM dbo.Contributions2(@fd, @td, @campusid, NULL, @nontaxded, @includeUnclosed) c2
	JOIN dbo.BundleHeader h ON c2.BundleHeaderId = h.BundleHeaderId
	GROUP BY c2.FundId, FundName, 
		CASE WHEN BundleHeaderTypeId = 4 THEN 1 ELSE 0 END,
		BundleHeaderTypeId
)
	SELECT 
		tt.COUNT ,
        tt.Amount ,
        tt.FundId ,
        tt.FundName ,
        tt.ONLINE ,
		bht.DESCRIPTION BundleType,
        f.FundAccountCode ,
        f.FundIncomeDept ,
        f.FundIncomeAccount ,
        f.FundCashAccount 
	FROM totals tt
	JOIN lookup.BundleHeaderTypes bht ON tt.BundleHeaderTypeId = bht.Id
	JOIN dbo.ContributionFund f ON f.FundId = tt.FundId
END

GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
