CREATE PROCEDURE [dbo].[RecordAttendance]
( @orgId INT, @peopleId INT, @meetingDate DateTime, @present BIT, @location nvarchar(50), @userid INT )
AS
BEGIN
-- Check to see if meeting exists and create it if not
-- Do this outside the transaction scope to prevent deadlocks
	DECLARE  @MeetingId INT
	EXEC @MeetingId = dbo.CreateMeeting @orgid, @meetingDate

-- now we know the meeting exists
	EXEC dbo.RecordAttend @MeetingId, @peopleid, @present
END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
