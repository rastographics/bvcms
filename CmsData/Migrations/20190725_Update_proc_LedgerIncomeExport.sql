ALTER PROC [dbo].[LedgerIncomeExport]
(
	@fd DATETIME, 
	@td DATETIME,
	@campusid INT,
	@nontaxded BIT,
	@includeUnclosed BIT,
	@includeBundleType BIT
)
AS
BEGIN
	DECLARE @T TABLE (
			[COUNT] int,
			Amount numeric,
			FundId int,
			FundName nvarchar,
			[ONLINE] BIT,
			BundleType int,
			FundAccountCode int,
			FundIncomeDept nvarchar,
			FundIncomeAccount nvarchar,
			FundCashAccount nvarchar);


	WITH totals AS (
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
	INSERT INTO @T (
			[COUNT],
			Amount,
			FundId,
			FundName,
			[ONLINE],
			BundleType,
			FundAccountCode,
			FundIncomeDept,
			FundIncomeAccount,
			FundCashAccount)
		SELECT 
			tt.[COUNT] ,
			tt.Amount ,
			tt.FundId ,
			tt.FundName ,
			tt.[ONLINE] ,
			bht.[Description] BundleType,
			f.FundAccountCode ,
			f.FundIncomeDept ,
			f.FundIncomeAccount ,
			f.FundCashAccount 
		FROM totals tt
		JOIN lookup.BundleHeaderTypes bht ON tt.BundleHeaderTypeId = bht.Id
		JOIN dbo.ContributionFund f ON f.FundId = tt.FundId
		ORDER BY f.FundId, bht.Description;

	IF @includeBundleType = 1
		SELECT * FROM @T
	ELSE
		SELECT [COUNT], Amount, FundId, FundName, [ONLINE], BundleType, FundAccountCode, FundIncomeDept, FundIncomeAccount, FundCashAccount FROM @T
	END
GO
