
CREATE PROC [dbo].[DonorTotalSummaryByAge]
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
		[Total<=25] MONEY,
		[Units<=25] INT,
		[Total<=35] MONEY,
		[Units<=35] INT,
		[Total<=50] MONEY,
		[Units<=50] INT,
		[Total<=65] MONEY,
		[Units<=65] INT,
		[Total>65] MONEY,
		[Units>65] INT,
		[Total=0] MONEY,
		[Units=0] INT
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
		SELECT SUM(Amount), COUNT(*), age FROM
		(
			SELECT
				c.FamilyId,
				Amount, 
				CASE WHEN ISNULL(p.Age, 0) = 0 THEN 0 
					 WHEN p.Age -@n <= 25 THEN 25 
					 WHEN p.Age -@n <= 35 THEN 35 
					 WHEN p.Age -@n <= 50 THEN 50 
					 WHEN p.Age -@n <= 65 THEN 65 
					 ELSE 100 
				END age
			FROM dbo.Contributions2(@fd, @td, @campus, 0, NULL, 1) c
			JOIN #ff ON #ff.FundId = c.FundId
			JOIN dbo.Families f ON f.FamilyId = c.FamilyId
			JOIN dbo.People p ON p.PeopleId = f.HeadOfHouseholdId
			WHERE Amount > 0
			AND (@fund = 0 OR @fund = c.FundId)
		) tt
		GROUP BY FamilyId, age


		INSERT @ts
		SELECT @td,
			dbo.DonorTotalGifts(@t, 25), 
			dbo.DonorTotalUnits(@t, 25),
			dbo.DonorTotalGifts(@t, 35), 
			dbo.DonorTotalUnits(@t, 35),
			dbo.DonorTotalGifts(@t, 50), 
			dbo.DonorTotalUnits(@t, 50),
			dbo.DonorTotalGifts(@t, 65), 
			dbo.DonorTotalUnits(@t, 65),
			dbo.DonorTotalGifts(@t, 100), 
			dbo.DonorTotalUnits(@t, 100),
			dbo.DonorTotalGifts(@t, 0), 
			dbo.DonorTotalUnits(@t, 0)

	    SET @n = @n + 1
	END
	SELECT * FROM @ts
	
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
