
CREATE FUNCTION [dbo].[GetScheduleDesc]
(
	@meetingtime DATETIME
)
RETURNS nvarchar(20)
AS 
BEGIN
	DECLARE @Ret nvarchar(20)
	DECLARE @MinDate11 DATETIME = CONVERT(DATETIME, 10)
	DECLARE @MinDate10 DATETIME = CONVERT(DATETIME, 9)
	DECLARE @time nvarchar(20) = REPLACE(LTRIM((SUBSTRING(CONVERT(nvarchar(20), ISNULL(@meetingtime, '8:00 AM'), 22), 10, 20))), ':00 ', ' ')
	SELECT @Ret = CASE CONVERT(DATE, ISNULL(@meetingtime, CONVERT(DATETIME, 10)))
					  WHEN @MinDate11 THEN 'None'
					  WHEN @MinDate10 THEN 'Any ' + @time
					  ELSE dbo.GetWeekDayNameOfDate(@meetingtime) + ' ' + @time
				  END
		   
    RETURN @Ret
END

GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
