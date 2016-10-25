CREATE FUNCTION [dbo].[GetAttendedTodaysMeeting]
    (
      @orgid INT ,
      @thisday INT,
      @pid INT
    )
RETURNS BIT
AS 
    BEGIN
        IF @thisday IS NULL
			SELECT @thisday = DATEPART(dw, GETDATE()) - 1

		DECLARE @attended BIT, @meetingid INT
		
		SELECT @meetingid = dbo.GetTodaysMeetingId(@orgid, @thisday)
		
		IF @meetingid IS NOT NULL
			SELECT @attended = AttendanceFlag FROM dbo.Attend 
			WHERE MeetingId = @meetingid AND PeopleId = @pid
		IF (@attended IS NULL)
			SELECT @attended = 0

        RETURN @attended
    END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
