ALTER FUNCTION [dbo].[PledgesSummary] ( @pid INT )
RETURNS
@pledgesSummary TABLE (FundId INT NOT NULL, FundName NVARCHAR(max), AmountPledged DECIMAL(38,2) NOT NULL, AmountContributed DECIMAL(38,2), Balance DECIMAL(38,2) NOT NULL)
AS
BEGIN
	DECLARE @spouseId INT;
	DECLARE @contributionOptionsId INT;

	SELECT 
		@spouseId = SpouseId,
		@contributionOptionsId = ContributionOptionsId
	FROM People WHERE PeopleId = @pid;

	IF (@contributionOptionsId <> 2 OR @spouseId IS NULL)
		SET @spouseId = NULL;

	with summary as (
		SELECT cf.FundId, cf.FundName, SUM(c.ContributionAmount) AmountPledged
		FROM Contribution c
		JOIN ContributionFund cf ON cf.FundId = c.FundId
		JOIN lookup.ContributionType ct ON c.ContributionTypeId = ct.Id 
		WHERE ct.Description = 'Pledge'
		AND c.PeopleId IN (@pid, @spouseId)
		GROUP BY cf.FundId, cf.FundName, ct.Description
	)
	INSERT INTO @pledgesSummary
	SELECT s.FundId, s.FundName, s.AmountPledged, 0, s.AmountPledged 
	FROM summary s

	UPDATE @pledgesSummary
	SET AmountContributed = con.AmountContributed,
	Balance = IIF(ps.AmountPledged - con.AmountContributed < 0, 0, ps.AmountPledged - con.AmountContributed)
	FROM
		(SELECT cf.FundId, cf.FundName, SUM(c.ContributionAmount) AmountContributed
		FROM Contribution c
		JOIN ContributionFund cf ON cf.FundId = c.FundId
		JOIN lookup.ContributionType ct ON c.ContributionTypeId = ct.Id 
		WHERE ct.Description <> 'Pledge'
		AND c.PeopleId IN (@pid, @spouseId)
		GROUP BY cf.FundId, cf.FundName) con
	JOIN @pledgesSummary ps on ps.FundId = con.FundId

	RETURN
END
GO
