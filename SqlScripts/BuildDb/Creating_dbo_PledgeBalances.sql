CREATE FUNCTION [dbo].[PledgeBalances] ( @fundid INT )
RETURNS TABLE 
AS
RETURN 
(
	SELECT
		c.CreditGiverId
		,SpouseId = c.CreditGiverId2
		,SUM(PledgeAmount) PledgeAmt
		,SUM(ISNULL(Amount, 0)) GivenAmt
		,CASE WHEN SUM(ISNULL(c.PledgeAmount, 0)) > 0 
				THEN SUM(ISNULL(PledgeAmount, 0)) - SUM(ISNULL(Amount, 0))
				ELSE 0
		 END Balance
	FROM Contributions2('1/1/1', '1/1/3000', 0, NULL, NULL, 1) c
	WHERE c.FundId = @fundid
	GROUP BY c.CreditGiverId, c.CreditGiverId2
)

GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
