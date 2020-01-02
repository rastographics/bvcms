ALTER PROC [dbo].[DonorTotalSummary]
(
	@enddt DATETIME,
	@years INT,
	@medianMin MONEY,
	@fund INT,
	@campus INT,
	@fundids VARCHAR(MAX)
)
AS
BEGIN
DECLARE @ts TABLE 
(
	ToDate DATETIME,

	Total MONEY,
	Units INT,
	Mean MONEY,
	Median MONEY,

	MemberTotal MONEY,
	MemberUnits INT,
	MemberMean MONEY,
	MemberMedian MONEY,

	NonMemberTotal MONEY,
	NonMemberUnits INT,
	NonMemberMean MONEY,
	NonMemberMedian MONEY
)

	DROP TABLE IF EXISTS #ff
	CREATE TABLE #ff ( FundId INT PRIMARY KEY )
	IF @fundids IS NOT NULL 
		INSERT #ff (FundId) SELECT Value FROM dbo.SplitInts(@fundids)
	ELSE
		INSERT #ff (FundId) SELECT FundId FROM dbo.ContributionFund

	DECLARE @t DonorTotalsTable
	DECLARE @n INT
	SET @n = 0
	WHILE (@n < @years)
	BEGIN
		DECLARE @td DATETIME = DATEADD(yy, -@n, CONVERT(DATE, @enddt))
		DECLARE @fd DATETIME = DATEADD(dd, 1, DATEADD(yy, -1, @td))

		DELETE @t
		INSERT INTO @t
		SELECT SUM(Amount), COUNT(*), memb FROM
		(
			SELECT
				c.FamilyId,
				Amount, 
				CASE WHEN p.MemberStatusId = 10 AND c.Date >= JoinDate THEN 1 ELSE 0 END memb
			FROM dbo.Contributions2(@fd, @td, @campus, 0, NULL, 1, NULL) c
			JOIN #ff f ON f.FundId = c.FundId
			JOIN dbo.People p ON p.PeopleId = c.PeopleId
			WHERE Amount > 0
			AND (c.FundId = @fund OR @fund = 0)
		) tt
		GROUP BY FamilyId, memb

		INSERT @ts
		SELECT @td,

			dbo.DonorTotalGifts(@t, NULL) [Total], 
			dbo.DonorTotalUnits(@t, NULL) [Units], 
			dbo.DonorTotalMean(@t, NULL) [Mean], 
			dbo.DonorTotalMedian(@t, NULL, @medianMin) [Median],

			dbo.DonorTotalGifts(@t, 1) [MemberTotal], 
			dbo.DonorTotalUnits(@t, 1) [MemberUnits], 
			dbo.DonorTotalMean(@t, 1) [MemberMean], 
			dbo.DonorTotalMedian(@t, 1, @medianMin) [MemberMedian],

			dbo.DonorTotalGifts(@t, 0) [NonMemberTotal], 
			dbo.DonorTotalUnits(@t, 0) [NonMemberUnits], 
			dbo.DonorTotalMean(@t, 0) [NonMemberMean], 
			dbo.DonorTotalMedian(@t, 0, @medianMin) [NonMemberMedian]

	    SET @n = @n + 1
	END
	SELECT * FROM @ts
END
GO

ALTER PROC [dbo].[DonorTotalSummaryBySize]
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
		FROM dbo.Contributions2(@fd, @td, @campus, 0, NULL, 1, NULL) c
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

ALTER PROC [dbo].[DonorTotalSummaryByAge]
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
			FROM dbo.Contributions2(@fd, @td, @campus, 0, NULL, 1, NULL) c
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

ALTER PROC [dbo].[DonorTotalSummary]
(
	@enddt DATETIME,
	@years INT,
	@medianMin MONEY,
	@fund INT,
	@campus INT,
	@fundids VARCHAR(MAX)
)
AS
BEGIN
DECLARE @ts TABLE 
(
	ToDate DATETIME,

	Total MONEY,
	Units INT,
	Mean MONEY,
	Median MONEY,

	MemberTotal MONEY,
	MemberUnits INT,
	MemberMean MONEY,
	MemberMedian MONEY,

	NonMemberTotal MONEY,
	NonMemberUnits INT,
	NonMemberMean MONEY,
	NonMemberMedian MONEY
)

	DROP TABLE IF EXISTS #ff
	CREATE TABLE #ff ( FundId INT PRIMARY KEY )
	IF @fundids IS NOT NULL 
		INSERT #ff (FundId) SELECT Value FROM dbo.SplitInts(@fundids)
	ELSE
		INSERT #ff (FundId) SELECT FundId FROM dbo.ContributionFund

	DECLARE @t DonorTotalsTable
	DECLARE @n INT
	SET @n = 0
	WHILE (@n < @years)
	BEGIN
		DECLARE @td DATETIME = DATEADD(yy, -@n, CONVERT(DATE, @enddt))
		DECLARE @fd DATETIME = DATEADD(dd, 1, DATEADD(yy, -1, @td))

		DELETE @t
		INSERT INTO @t
		SELECT SUM(Amount), COUNT(*), memb FROM
		(
			SELECT
				c.FamilyId,
				Amount, 
				CASE WHEN p.MemberStatusId = 10 AND c.Date >= JoinDate THEN 1 ELSE 0 END memb
			FROM dbo.Contributions2(@fd, @td, @campus, 0, NULL, 1, NULL) c
			JOIN #ff f ON f.FundId = c.FundId
			JOIN dbo.People p ON p.PeopleId = c.PeopleId
			WHERE Amount > 0
			AND (c.FundId = @fund OR @fund = 0)
		) tt
		GROUP BY FamilyId, memb

		INSERT @ts
		SELECT @td,

			dbo.DonorTotalGifts(@t, NULL) [Total], 
			dbo.DonorTotalUnits(@t, NULL) [Units], 
			dbo.DonorTotalMean(@t, NULL) [Mean], 
			dbo.DonorTotalMedian(@t, NULL, @medianMin) [Median],

			dbo.DonorTotalGifts(@t, 1) [MemberTotal], 
			dbo.DonorTotalUnits(@t, 1) [MemberUnits], 
			dbo.DonorTotalMean(@t, 1) [MemberMean], 
			dbo.DonorTotalMedian(@t, 1, @medianMin) [MemberMedian],

			dbo.DonorTotalGifts(@t, 0) [NonMemberTotal], 
			dbo.DonorTotalUnits(@t, 0) [NonMemberUnits], 
			dbo.DonorTotalMean(@t, 0) [NonMemberMean], 
			dbo.DonorTotalMedian(@t, 0, @medianMin) [NonMemberMedian]

	    SET @n = @n + 1
	END
	SELECT * FROM @ts
END
GO
