CREATE FUNCTION [dbo].[AvgSunAttendance]()
RETURNS int
AS
	BEGIN
	
	DECLARE @a INT
	
	SELECT @a = AVG(cnt)
	FROM
		(
		SELECT dbo.SundayForDate(MeetingDate) sun, SUM(NumPresent) cnt 
		FROM Meetings
		WHERE DATEPART(dw, MeetingDate) = 1 and MeetingDate > DATEADD(d, -365, GETDATE())
		GROUP BY dbo.SundayForDate(MeetingDate)
	) tt	

	RETURN @a
	
END


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
