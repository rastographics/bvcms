CREATE FUNCTION [dbo].[GivingChangeFund] (@days INT, @fundid INT)
RETURNS TABLE 
AS
RETURN 
(
	WITH period1 AS (
		SELECT p1.PeopleId, SUM(c1.Amount) c1amt, 0 c2amt
		FROM dbo.People p1
		JOIN dbo.Contributions2(
			DATEADD(DAY, -@days * 2, GETDATE()) -- start date
			,DATEADD(DAY, -@days, GETDATE()) -- enddate
			,0, 0, 0, 1) c1 ON p1.PeopleId = c1.CreditGiverId AND c1.FundId = @fundid
		GROUP BY p1.PeopleId
	), 
	period2 AS (
		SELECT p2.PeopleId, 0 c1amt, SUM(c2.Amount) c2amt
		FROM dbo.People p2
		JOIN dbo.Contributions2(
			DATEADD(DAY, -@days, GETDATE()) -- start date
			,GETDATE() -- end date
			,0, 0, 0, 1) c2 ON p2.PeopleId = c2.CreditGiverId AND c2.FundId = @fundid
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
		f.FundName,
		PeopleId, 
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
	JOIN dbo.ContributionFund f ON f.FundId = @fundid
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
