CREATE FUNCTION [dbo].[DonorTotalUnitsSize](@t DonorTotalsTable READONLY, @min INT, @max INT)
RETURNS MONEY
AS
BEGIN
	DECLARE @ret MONEY
	SELECT @ret = COUNT(*) FROM @t WHERE tot > (@min - 1) AND tot <= @max
	RETURN @ret
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
