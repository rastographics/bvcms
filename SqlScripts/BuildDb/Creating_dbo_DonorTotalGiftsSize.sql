CREATE FUNCTION [dbo].[DonorTotalGiftsSize](@t DonorTotalsTable READONLY, @min INT, @max INT)
RETURNS MONEY
AS
BEGIN
	DECLARE @ret MONEY
	SELECT @ret = SUM(tot) FROM @t WHERE tot > (@min - 1) AND tot <= @max
	RETURN @ret
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
