CREATE FUNCTION [dbo].[LastAttended] (@orgid INT, @pid INT)
RETURNS DATETIME
AS
	BEGIN
	DECLARE @dt DATETIME
		SELECT @dt = MAX(m.MeetingDate) FROM dbo.Attend a
		JOIN dbo.Meetings m
		ON a.MeetingId = m.MeetingId
		WHERE a.AttendanceFlag = 1 AND m.OrganizationId = @orgid AND a.PeopleId = @pid
	RETURN @dt
	END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
