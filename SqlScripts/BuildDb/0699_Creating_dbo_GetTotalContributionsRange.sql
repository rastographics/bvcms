
CREATE FUNCTION [dbo].[GetTotalContributionsRange]
(
	@fd DATETIME, 
	@td DATETIME,
	@campusid INT,
	@nontaxded BIT,
	@includeUnclosed BIT
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT SUM(tt.Amount) AS Total, SUM(tt.Count) AS [Count], tt.Range
	FROM ( SELECT SUM(Amount) AS Amount, 1 AS [Count],
		   CASE WHEN SUM(Amount) <= 100 THEN 100
			    WHEN SUM(Amount) <= 250 THEN 250
			    WHEN Sum(Amount) <= 500 THEN 500
			    WHEN Sum(Amount) <= 750 THEN 750
			    WHEN Sum(Amount) <= 1000 THEN 1000
			    WHEN Sum(Amount) <= 2000 THEN 2000
			    WHEN Sum(Amount) <= 3000 THEN 3000
			    WHEN Sum(Amount) <= 4000 THEN 4000
			    WHEN Sum(Amount) <= 5000 THEN 5000
			    WHEN Sum(Amount) <= 10000 THEN 10000
			    WHEN Sum(Amount) <= 20000 THEN 20000
			    WHEN Sum(Amount) <= 30000 THEN 30000
			    WHEN Sum(Amount) <= 40000 THEN 40000
			    WHEN Sum(Amount) <= 50000 THEN 50000
			    WHEN Sum(Amount) <= 100000 THEN 100000
			    ELSE 1000000
		   END AS [Range]
		   FROM dbo.Contributions2(@fd, @td, @campusid, NULL, @nontaxded, @includeUnclosed)
		   GROUP BY CreditGiverId
		  ) tt
	GROUP BY tt.Range
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
