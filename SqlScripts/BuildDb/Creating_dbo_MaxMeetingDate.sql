CREATE FUNCTION [dbo].[MaxMeetingDate] (@oid INT)
RETURNS DATETIME
AS
BEGIN
	DECLARE @dt DATETIME
		SELECT @dt = MAX(MeetingDate) 
		FROM dbo.Meetings
		WHERE ISNULL(NumPresent, HeadCount) > 0
		AND MeetingDate <= dbo.MaxPastMeeting()
		AND OrganizationId = @oid
	RETURN @dt
END

GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
