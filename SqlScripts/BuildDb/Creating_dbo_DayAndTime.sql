-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[DayAndTime] (@dt DATETIME)
RETURNS nvarchar(20)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @daytime nvarchar(20)

SELECT @daytime =
	CASE DATEPART(dw,@dt)
    WHEN 1 THEN 'Sunday'
    WHEN 2 THEN 'Monday'
    WHEN 3 THEN 'Tuesday'
    WHEN 4 THEN 'Wednesday'
    WHEN 5 THEN 'Thursday'
    WHEN 6 THEN 'Friday'
    WHEN 7 THEN 'Saturday'
    END + ' ' + SUBSTRING(CONVERT(nvarchar,@dt,0),13,7)
    
	RETURN @daytime

END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
