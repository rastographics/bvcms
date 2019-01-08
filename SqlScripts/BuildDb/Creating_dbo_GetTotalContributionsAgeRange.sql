CREATE FUNCTION [dbo].[GetTotalContributionsAgeRange]
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
	WITH ranges AS (
		SELECT 
			SUM(c.ContributionAmount) Amount,
			COUNT(c.PeopleId) [Count],
			CASE WHEN p.Age IS NULL THEN 0
				 WHEN p.age < 10 THEN 1
				 ELSE ISNULL(p.Age, 0) - (ISNULL(p.Age, 0) % 10)
			END [Range]
		FROM dbo.ContributionSearch(NULL, NULL, NULL, NULL, @fd, @td, @campusid, 0, 2, 0, 
				CASE WHEN ISNULL(@nontaxded, 0) = 1 THEN 'nontaxded' ELSE 'taxded' END,
				NULL, 0, NULL, @includeUnclosed, NULL, NULL, NULL, @fundids) cs
		JOIN dbo.Contribution c ON c.ContributionId = cs.ContributionId
		JOIN dbo.People p ON p.PeopleId = c.PeopleId
		GROUP BY c.PeopleId, p.Age
	)
	SELECT 
		SUM(r.Amount) Total, 
		SUM(r.[Count]) [Count],  
		COUNT(*) DonorCount,
		CASE WHEN r.[Range] = 0 THEN '0' 
	         WHEN r.[Range] = 1 THEN '1 - 9' 
		     ELSE FORMAT(r.[Range], 'n0') + ' - ' + FORMAT(r.[Range] + 9, 'n0')
		END [Range]
	FROM ranges r
	GROUP BY r.[Range]
)

GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
