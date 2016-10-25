CREATE FUNCTION [dbo].[DonorTotalUnitsAttrRange](@t DonorTotalsTable READONLY, @min INT, @max INT)
RETURNS INT
AS
BEGIN
	DECLARE @ret INT
	SELECT @ret = COUNT(*) FROM @t WHERE attr > (@min - 1) AND attr <= @max
	RETURN @ret
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
