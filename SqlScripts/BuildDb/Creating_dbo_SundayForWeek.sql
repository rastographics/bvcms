-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[SundayForWeek](@year INT, @week INT)
RETURNS datetime
AS
BEGIN

DECLARE @dt DATETIME 
SELECT @dt = DATEADD(MONTH,((@year-1900)*12),0) -- jan 1 for year
SELECT @dt = DATEADD(MONTH, 9, @dt) -- Oct 1 for year
SELECT @dt = DATEADD(d, -DATEPART(dw, @dt)+1, @dt) -- sunday of that week
IF DATEPART(MONTH, @dt) < 10 -- are we in september now?
	SELECT @dt = DATEADD(d, 7, @dt) -- next sunday (to get into october)
SELECT @dt = DATEADD(ww, @week - 1, @dt) -- sunday for week number

	-- Return the result of the function
	RETURN @dt

END
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
