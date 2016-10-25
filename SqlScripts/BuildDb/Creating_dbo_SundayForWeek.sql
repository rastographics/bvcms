CREATE FUNCTION [dbo].[SundayForWeek](@year INT, @week INT)
RETURNS datetime
AS
BEGIN

DECLARE @wkn INT = @year * 100 + @week - 1
RETURN dbo.SundayForWeekNumber(@wkn)



END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
