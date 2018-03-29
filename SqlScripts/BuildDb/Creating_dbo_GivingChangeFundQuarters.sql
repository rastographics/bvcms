CREATE FUNCTION [dbo].[GivingChangeFundQuarters] (@fd DATETIME, @td DATETIME, @fundids VARCHAR(MAX), @tagid INT)
RETURNS TABLE 
AS
RETURN 
(
WITH period1 AS (
		SELECT p1.PeopleId, SUM(c1.Amount) c1amt, 0 c2amt
		FROM dbo.People p1
		JOIN dbo.Contributions2(
			DATEADD(YEAR, -1, @fd) -- start date
			,DATEADD(YEAR, -1, @td) -- enddate
			,0, 0, 0, 1) c1 ON p1.PeopleId = c1.CreditGiverId 
		WHERE @fundids IS NULL OR EXISTS(SELECT NULL FROM dbo.SplitInts(@fundids) WHERE Value = c1.FundId)
		GROUP BY p1.PeopleId
	), 
	period2 AS (
		SELECT p2.PeopleId, 0 c1amt, SUM(c2.Amount) c2amt
		FROM dbo.People p2
		JOIN dbo.Contributions2(
			@fd ,@td
			,0, 0, 0, 1) c2 ON p2.PeopleId = c2.CreditGiverId 
		WHERE (@fundids IS NULL OR EXISTS(SELECT NULL FROM dbo.SplitInts(@fundids) WHERE Value = c2.FundId))

		GROUP BY p2.PeopleId
	), 
	bothperiods AS (
		SELECT PeopleId, c1amt, c2amt FROM period1
		UNION
		SELECT PeopleId, c1amt, c2amt FROM period2
	),
	grouped AS (
		SELECT 
			PeopleId 
			,TotalPeriod1 = ISNULL(SUM(c1amt), 0)
			,TotalPeriod2 = ISNULL(SUM(c2amt), 0)
		FROM bothperiods GROUP BY PeopleId
	), 
	getpercent AS (
		SELECT 
			PeopleId, TotalPeriod1, TotalPeriod2, 
			PctOfPrevious = (NULLIF(TotalPeriod2, 0) / NULLIF(TotalPeriod1, 0)) * 100
		FROM grouped
	) 
	SELECT 
		pc.PeopleId, 
		TotalPeriod1, 
		TotalPeriod2, 
		PctChange = CASE 
			WHEN PctOfPrevious IS NULL AND TotalPeriod1 = 0 THEN 100000.0
			WHEN PctOfPrevious IS NULL AND TotalPeriod2 = 0 THEN -100000.0
			ELSE PctOfPrevious - 100.0
		END,
		Change = CASE 
			WHEN PctOfPrevious IS NULL AND TotalPeriod1 = 0 THEN 'Started'
			WHEN PctOfPrevious IS NULL AND TotalPeriod2 = 0 THEN 'Stopped'
			WHEN PctOfPrevious > 100 THEN FORMAT(-(TotalPeriod1 - TotalPeriod2) / TotalPeriod1, '0%') + ' Increase'
			WHEN PctOfPrevious < 100 THEN FORMAT((TotalPeriod1 - TotalPeriod2) / TotalPeriod1, '0%') + ' Decrease'
			ELSE 'No Change'
		END
	FROM getpercent pc
	JOIN dbo.People p ON p.PeopleId = pc.PeopleId
	WHERE (@tagid IS NULL OR EXISTS(SELECT NULL FROM dbo.TagPerson tp WHERE tp.Id = @tagid AND tp.PeopleId = pc.PeopleId))
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
