CREATE FUNCTION [dbo].[GetContributionsRange]
(
	@fd DATETIME, 
	@td DATETIME,
	@campusid INT,
	@nontaxded BIT,
	@includeUnclosed BIT,
	@pledge BIT,
	@fundid INT,
	@fundids VARCHAR(MAX)
)
RETURNS TABLE 
AS
RETURN 
(
	WITH contributions AS (
	    SELECT Amount = c.ContributionAmount, c.PeopleId ,
			   Range = CASE
					WHEN ISNULL(c.ContributionAmount, 0) < 101 THEN 1
					WHEN c.ContributionAmount < 251 THEN 2
					WHEN c.ContributionAmount < 501 THEN 3
					WHEN c.ContributionAmount < 751 THEN 4
					WHEN c.ContributionAmount < 1001 THEN 5
					WHEN c.ContributionAmount < 2001 THEN 6
					WHEN c.ContributionAmount < 3001 THEN 7
					WHEN c.ContributionAmount < 4001 THEN 8
					WHEN c.ContributionAmount < 5001 THEN 9
					WHEN c.ContributionAmount < 10001 THEN 10
					WHEN c.ContributionAmount < 20001 THEN 11
					WHEN c.ContributionAmount < 30001 THEN 12
					WHEN c.ContributionAmount < 40001 THEN 13
					WHEN c.ContributionAmount < 50001 THEN 14
					WHEN c.ContributionAmount < 100001 THEN 15
					ELSE 16
				END
		FROM dbo.ContributionSearch(NULL, NULL, NULL, NULL, @fd, @td, @campusid, @fundid, 2, 0, 
				CASE WHEN ISNULL(@nontaxded, 0) = 1 THEN 'nontaxded' WHEN @pledge = 1 THEN 'pledge' ELSE 'taxded' END, 
				0, 0, NULL, @includeUnclosed, NULL, NULL, NULL, @fundids) cs
		JOIN dbo.Contribution c ON c.ContributionId = cs.ContributionId
	),
	sumpeoplerange AS (
		SELECT 
			Total = SUM(cc.Amount),
			[Count] = COUNT(*),
			cc.[Range]
		FROM contributions cc
		GROUP BY cc.[Range], cc.PeopleId
	)
	SELECT 
		s.[Range],
		DonorCount = COUNT(*),
		[Count] = SUM(s.Count),
		Total = SUM(s.Total)
	FROM sumpeoplerange s
	GROUP BY s.[Range]
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
