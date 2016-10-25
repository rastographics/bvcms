CREATE FUNCTION [dbo].[DonorTotalMean](@t DonorTotalsTable READONLY, @attr INT = NULL)
RETURNS MONEY
AS
BEGIN
	DECLARE @ret MONEY
	SELECT @ret = AVG(tot) FROM @t WHERE attr = @attr OR @attr IS NULL
	RETURN @ret
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
