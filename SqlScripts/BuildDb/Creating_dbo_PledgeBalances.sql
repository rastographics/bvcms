CREATE FUNCTION [dbo].[PledgeBalances] ( @fundid INT )
RETURNS TABLE 
AS
RETURN 
(
	SELECT
		c.CreditGiverId
		,c.SpouseId
		,SUM(PledgeAmount) PledgeAmt
		,CASE WHEN SUM(ISNULL(c.PledgeAmount, 0)) > 0 
				THEN SUM(ISNULL(PledgeAmount, 0)) - SUM(ISNULL(Amount, 0))
				ELSE 0
		 END Balance
	FROM Contributions2('1/1/1', '1/1/3000', 0, NULL, NULL, 1) c
	WHERE c.FundId = @fundid
	GROUP BY c.CreditGiverId, c.SpouseId
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
