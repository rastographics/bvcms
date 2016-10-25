CREATE FUNCTION [dbo].[PledgeReport]
(
	@fd DATETIME, 
	@td DATETIME,
	@campusid INT
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT tt.FundId, tt.FundName, SUM(Plg) AS Plg, 
		SUM(CASE WHEN tt.Plg > 0 THEN tt.Amt END) AS ToPledge,
		SUM(CASE WHEN tt.Plg = 0 THEN tt.Amt END) AS NotToPledge,
		SUM(tt.Amt) AS ToFund
	FROM
	(
	SELECT SUM(ISNULL(Amount,0)) Amt, SUM(ISNULL(PledgeAmount, 0)) Plg, FundId, FundName
	FROM Contributions2(@fd, @td, @campusid, NULL, NULL, 1)
	GROUP BY CreditGiverId, SpouseId, FundId, FundName, OpenPledgeFund
	HAVING  OpenPledgeFund = 1
	) tt
	GROUP BY FundId, tt.FundName
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
