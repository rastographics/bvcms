CREATE FUNCTION [dbo].[GetTotalContributionsAgeRange]
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
	SELECT 
		SUM(tt.Amount) AS Total, 
		SUM(tt.Count) AS [Count], 
		COUNT(*) AS DonorCount,
		CASE WHEN ISNULL(tt.[Range], 0) = 0 THEN '0' ELSE CAST(tt.[Range] * 10 - 9 AS VARCHAR) + ' - ' + CAST(tt.[Range] * 10 AS VARCHAR) END AS [Range]
	FROM ( SELECT SUM(Amount) AS Amount, 1 AS [Count],
		   CASE WHEN ISNULL(p.Age, 0) = 0 THEN 0 ELSE p.Age / 10 + 1 END AS [Range]
		   FROM dbo.Contributions2(@fd, @td, @campusid, NULL, @nontaxded, @includeUnclosed)
		   JOIN dbo.People p ON p.PeopleId = Contributions2.CreditGiverId
		   GROUP BY CreditGiverId, Age
		  ) tt
	GROUP BY tt.Range
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
