CREATE FUNCTION [dbo].[AgeInMonths](@pid int, @asof DATETIME)
RETURNS int
AS
BEGIN
	DECLARE @bd DATETIME = dbo.Birthday(@pid)
	DECLARE @mos INT
		= DATEDIFF(MONTH, @bd, @asof) - IIF(DATEPART(DAY, @asof) < DATEPART(DAY, @bd), 1, 0)
	RETURN @mos
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
