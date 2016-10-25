CREATE FUNCTION [dbo].[DonorTotalGiftsAttrRange](@t DonorTotalsTable READONLY, @min INT, @max INT)
RETURNS MONEY
AS
BEGIN
	DECLARE @ret MONEY
	SELECT @ret = SUM(tot) FROM @t WHERE attr > (@min - 1) AND attr <= @max
	RETURN @ret
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
