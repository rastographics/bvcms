CREATE FUNCTION [dbo].[DonorTotalUnits](@t DonorTotalsTable READONLY, @attr INT = NULL)
RETURNS INT
AS
BEGIN
	DECLARE @ret INT
	SELECT @ret = COUNT(*) FROM @t WHERE attr = @attr OR @attr IS NULL
	RETURN @ret
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
