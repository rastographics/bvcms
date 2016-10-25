-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[SundayForWeekNumber](@wkn INT)
RETURNS datetime
AS
BEGIN

DECLARE @year INT = FLOOR(@wkn / 100), 
	@week INT = @wkn % 100

DECLARE @dt1 DATETIME = DATEFROMPARTS(@year, 1, 1)
DECLARE @dt DATETIME 
SELECT @dt = DATEADD(dd,-1,DATEADD(wk, DATEDIFF(wk,0,dateadd(dd,7-datepart(day,@dt1),@dt1)), 0))

SELECT @dt = DATEADD(ww, @week - 1, @dt) -- sunday for week number

	-- Return the result of the function
	RETURN @dt

END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
