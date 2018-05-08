CREATE PROC [dbo].[DonorTotalSummaryBySize]
(
	@enddt DATETIME,
	@years INT,
	@fund INT,
	@campus INT,
	@fundids VARCHAR(MAX)
)
AS
BEGIN

	DECLARE @ts TABLE 
	(
		[12 Mos as of] DATETIME,
		[Total<=1K] MONEY,
		[Units<=1K] INT,
		[Total<=5K] MONEY,
		[Units<=5K] INT,
		[Total<=15K] MONEY,
		[Units<=15K] INT,
		[Total<=25K] MONEY,
		[Units<=25K] INT,
		[Total<=50K] MONEY,
		[Units<=50K] INT,
		[Total<=100K] MONEY,
		[Units<=100K] INT,
		[Total>100K] MONEY,
		[Units>100K] INT
	)
	DROP TABLE IF EXISTS #ff
	CREATE TABLE #ff ( FundId INT PRIMARY KEY )
	IF @fundids IS NOT NULL 
		INSERT #ff (FundId) SELECT Value FROM dbo.SplitInts(@fundids)
	ELSE
		INSERT #ff (FundId) SELECT FundId FROM dbo.ContributionFund

	DECLARE @t DonorTotalsTable
	DECLARE @n INT = 0
	WHILE (@n < @years)
	BEGIN
		DECLARE @td DATETIME = DATEADD(yy, -@n, CONVERT(DATE, @enddt))
		DECLARE @fd DATETIME = DATEADD(dd, 1, DATEADD(yy, -1, @td))

		DELETE @t
		INSERT INTO @t
		SELECT SUM(Amount), COUNT(*), 1
		FROM dbo.Contributions2(@fd, @td, @campus, 0, NULL, 1) c
		JOIN #ff ON #ff.FundId = c.FundId
		WHERE (@fund = 0 OR @fund = c.FundId)
		AND Amount > 0
		GROUP BY FamilyId


		INSERT @ts
		SELECT @td,
			dbo.DonorTotalGiftsSize(@t, 1,1000), 
			dbo.DonorTotalUnitsSize(@t, 1,1000),
			dbo.DonorTotalGiftsSize(@t, 1000, 5000), 
			dbo.DonorTotalUnitsSize(@t, 1000, 5000),
			dbo.DonorTotalGiftsSize(@t, 5000, 15000), 
			dbo.DonorTotalUnitsSize(@t, 5000, 15000),
			dbo.DonorTotalGiftsSize(@t, 15000, 25000), 
			dbo.DonorTotalUnitsSize(@t, 15000, 25000),
			dbo.DonorTotalGiftsSize(@t, 25000, 50000), 
			dbo.DonorTotalUnitsSize(@t, 25000, 50000),
			dbo.DonorTotalGiftsSize(@t, 50000, 100000), 
			dbo.DonorTotalUnitsSize(@t, 50000, 100000),
			dbo.DonorTotalGiftsSize(@t, 100000, 1000000000), 
			dbo.DonorTotalUnitsSize(@t, 100000, 1000000000)

	    SET @n = @n + 1
	END
	SELECT * FROM @ts
	
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
