CREATE FUNCTION [dbo].[MinMeetingDate] (@oid INT, @pid INT, @yearago DATETIME)
RETURNS DATETIME
AS
BEGIN
	DECLARE @dt DATETIME = (
		SELECT MIN(MeetingDate)
		FROM dbo.Attend
		WHERE MeetingDate <= dbo.MaxPastMeeting()
		AND OrganizationId = @oid
		AND PeopleId = @pid)
	DECLARE @fm DATETIME = (
		SELECT FirstMeetingDate
		FROM dbo.Organizations
		WHERE OrganizationId = @oid)
	SET @dt = IIF(@dt > @fm, @dt, @fm)
	RETURN IIF(@dt > @yearago, @dt, @yearago)
END



GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
