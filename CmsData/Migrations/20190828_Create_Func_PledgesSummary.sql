GO
IF OBJECT_ID('[dbo].[PledgesSummary]') IS NOT NULL
DROP FUNCTION [dbo].[PledgesSummary] 
GO
CREATE FUNCTION [dbo].[PledgesSummary] ( @pid INT )
RETURNS
@pledgesSummary TABLE (FundId INT NOT NULL, FundName NVARCHAR(max), AmountPledged DECIMAL(38,2) NOT NULL, AmountContributed DECIMAL(38,2), Balance DECIMAL(38,2) NOT NULL)
AS
BEGIN
	DECLARE @contributed TABLE(FundId INT, FundName NVARCHAR(max), AmountContributed DECIMAL(38,2));

	INSERT INTO @pledgesSummary 
	SELECT cf.FundId, cf.FundName, SUM(c.ContributionAmount), 0, 0
	FROM Contribution c
	JOIN ContributionFund cf ON cf.FundId = c.FundId
	JOIN lookup.ContributionType ct ON c.ContributionTypeId = ct.Id 
	WHERE ct.Description = 'Pledge'
	AND c.PeopleId = @pid
	GROUP BY cf.FundId, cf.FundName, ct.Description;

	INSERT INTO @contributed
	SELECT cf.FundId, cf.FundName, SUM(c.ContributionAmount)
	FROM Contribution c
	JOIN ContributionFund cf ON cf.FundId = c.FundId
	JOIN lookup.ContributionType ct ON c.ContributionTypeId = ct.Id 
	WHERE ct.Description <> 'Pledge'
	AND c.PeopleId = @pid
	GROUP BY cf.FundId, cf.FundName, ct.Description

	UPDATE @pledgesSummary
	SET
		AmountContributed = c.AmountContributed,
		Balance = IIF(ps.AmountPledged - c.AmountContributed < 0, 0, ps.AmountPledged - c.AmountContributed)
	FROM @contributed c
	JOIN @pledgesSummary ps on ps.FundId = c.FundId

	RETURN
END
