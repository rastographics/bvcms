CREATE FUNCTION [dbo].[DonorTotalGifts](@t DonorTotalsTable READONLY, @attr INT)
RETURNS MONEY
AS
BEGIN
	DECLARE @ret MONEY
	SELECT @ret = SUM(tot) FROM @t WHERE attr = @attr OR @attr IS NULL
	RETURN @ret
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
