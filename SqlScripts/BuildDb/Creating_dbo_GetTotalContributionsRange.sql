CREATE FUNCTION [dbo].[GetTotalContributionsRange]
(
	@fd DATETIME, 
	@td DATETIME,
	@campusid INT,
	@nontaxded BIT,
	@includeUnclosed BIT,
	@fundids VARCHAR(MAX)
)
RETURNS TABLE 
AS
RETURN 
(
	WITH contributions AS (
	    SELECT Amount = c.ContributionAmount, c.PeopleId 
		FROM dbo.ContributionSearch(NULL, NULL, NULL, NULL, @fd, @td, @campusid, 0, 2, 0, 
				CASE WHEN ISNULL(@nontaxded, 0) = 1 THEN 'nontaxded' ELSE 'taxded' END,
				NULL, 0, NULL, @includeUnclosed, NULL, NULL, NULL, @fundids) cs
		JOIN dbo.Contribution c ON c.ContributionId = cs.ContributionId
	),
	sums AS (
		SELECT 
			SUM(Amount) AS SumAmt, 
			1 AS [Count],
		   CASE WHEN SUM(Amount) <= 100 THEN 100
			    WHEN SUM(Amount) <= 250 THEN 250
			    WHEN SUM(Amount) <= 500 THEN 500
			    WHEN SUM(Amount) <= 750 THEN 750
			    WHEN SUM(Amount) <= 1000 THEN 1000
			    WHEN SUM(Amount) <= 2000 THEN 2000
			    WHEN SUM(Amount) <= 3000 THEN 3000
			    WHEN SUM(Amount) <= 4000 THEN 4000
			    WHEN SUM(Amount) <= 5000 THEN 5000
			    WHEN SUM(Amount) <= 10000 THEN 10000
			    WHEN SUM(Amount) <= 20000 THEN 20000
			    WHEN SUM(Amount) <= 30000 THEN 30000
			    WHEN SUM(Amount) <= 40000 THEN 40000
			    WHEN SUM(Amount) <= 50000 THEN 50000
			    WHEN SUM(Amount) <= 100000 THEN 100000
			    ELSE 1000000
		   END AS [Range]
		   FROM contributions
	    GROUP BY PeopleId
	)
	SELECT SUM(sums.SumAmt) AS Total, SUM(sums.Count) AS [Count], sums.RANGE
	FROM sums
	GROUP BY sums.Range
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
