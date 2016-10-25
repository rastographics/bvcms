
CREATE FUNCTION [dbo].[GetWeekDayNameOfDate]
(
  @Date datetime
)
RETURNS nvarchar(50)
BEGIN

DECLARE @DayName nvarchar(50)

SELECT 
  @DayName =  
  CASE (DATEPART(dw, @Date) + @@DATEFIRST) %   7
    WHEN 1 THEN 'Sun'
    WHEN 2 THEN 'Mon'
    WHEN 3 THEN 'Tue'
    WHEN 4 THEN 'Wed'
    WHEN 5 THEN 'Thu'
    WHEN 6 THEN 'Fri'    
    WHEN 0 THEN 'Sat'
  END

RETURN ISNULL(@DayName, 'Sun')

END

GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
