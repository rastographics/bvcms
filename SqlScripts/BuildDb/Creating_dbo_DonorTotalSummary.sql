
CREATE PROC [dbo].[DonorTotalSummary]
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
			FROM dbo.Contributions2(@fd, @td, @campus, 0, NULL, 1) c
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
IF @@ERROR <> 0 SET NOEXEC ON
GO
