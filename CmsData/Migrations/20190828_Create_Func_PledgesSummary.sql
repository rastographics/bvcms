GO
IF OBJECT_ID('[dbo].[PledgesSummary]') IS NOT NULL
DROP FUNCTION [dbo].[PledgesSummary] 
GO
CREATE FUNCTION [dbo].[PledgesSummary] ( @pid INT )
RETURNS TABLE 
AS
RETURN 
(
	WITH Pledged(FundId, FundName, AmountPledged, ContributionDescription)
	AS
	(
		SELECT cf.FundId, cf.FundName, SUM(c.ContributionAmount), ct.Description
		FROM Contribution c
		JOIN ContributionFund cf ON cf.FundId = c.FundId
		JOIN lookup.ContributionType ct ON c.ContributionTypeId = ct.Id 
		WHERE ct.Description = 'Pledge'
		AND c.PeopleId = @pid
		GROUP BY cf.FundId, cf.FundName, ct.Description
	)
	,Contributed(FundId, FundName, AmountContributed) AS
	(
		SELECT cf.FundId, cf.FundName, SUM(c.ContributionAmount)
		FROM Contribution c
		JOIN ContributionFund cf ON cf.FundId = c.FundId
		JOIN lookup.ContributionType ct ON c.ContributionTypeId = ct.Id 
		WHERE ct.Description <> 'Pledge'
		AND c.PeopleId = @pid
		GROUP BY cf.FundId, cf.FundName, ct.Description
	)
	SELECT 
	ple.FundId AS FundId, 
	ple.FundName AS FundName, 
	ple.AmountPledged AS AmountPledged, 
	con.AmountContributed AS AmountContributed,  
	(IIF(ple.AmountPledged - con.AmountContributed < 0, 0, ple.AmountPledged - con.AmountContributed)) AS Balance
	FROM Pledged ple
	JOIN Contributed con ON con.FundId = ple.FundId
)
