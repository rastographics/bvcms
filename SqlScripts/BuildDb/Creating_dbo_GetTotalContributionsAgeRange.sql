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
	WITH ranges AS (
		SELECT 
			Amount = SUM(Amount), 
			[Count] = COUNT(*),
			[Range] = CASE WHEN ISNULL(p.Age, 0) = 0 THEN 0 ELSE p.Age / 10 + 1 END
		FROM dbo.Contributions2(@fd, @td, @campusid, NULL, @nontaxded, @includeUnclosed)
		JOIN dbo.People p ON p.PeopleId = Contributions2.CreditGiverId
		GROUP BY CreditGiverId, Age
	)
	SELECT 
		Total = SUM(r.Amount), 
		[Count] = SUM(r.COUNT), 
		DonorCount = COUNT(*),
		[Range] = CASE WHEN ISNULL(r.[Range], 0) = 0 THEN '0' 
					   ELSE FORMAT(r.[Range] * 10 - 9, 'n0') + ' - ' + FORMAT(r.[Range] * 10, 'n0')
				  END
	FROM ranges r
	GROUP BY r.[Range]
)

GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
