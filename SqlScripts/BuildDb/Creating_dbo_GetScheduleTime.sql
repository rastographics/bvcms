CREATE FUNCTION [dbo].[GetScheduleTime]
(
	@day INT,
	@time DATETIME
)
RETURNS DATETIME
AS 
    BEGIN
        DECLARE @Ret DATETIME
        DECLARE @Timeonly DATETIME = DATEADD(dd, -DATEDIFF(dd, 0, @time), @time)
		 
		DECLARE @MinDate DATETIME = CONVERT(DATETIME, 0)
		DECLARE @MinDayOfWeek INT = DATEPART(dw, @MinDate) - 1
		SELECT @day = ISNULL(@day, 0)
		IF (@MinDayOfWeek > @day)
			SELECT @day = @day + 7
		SELECT @Ret = CONVERT(DATETIME, @day - @MinDayOfWeek) + @Timeonly
		
        RETURN @Ret
    END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
