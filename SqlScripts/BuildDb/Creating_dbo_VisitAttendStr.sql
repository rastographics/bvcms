CREATE FUNCTION [dbo].[VisitAttendStr](@oid INT, @pid INT, @MeetingDay1 DATE, @MeetingDay2 DATE)
RETURNS VARCHAR(MAX)
AS
BEGIN
	DECLARE @ret VARCHAR(MAX)

	;WITH dts AS (
		SELECT DATEADD(WEEK, Number, @MeetingDay1) dt 
		FROM dbo.Numbers 
		WHERE DATEADD(WEEK, Number, @MeetingDay1) <= @MeetingDay2
	),
	attends AS (
		SELECT dbo.SundayForDate(MeetingDate) MeetingDay, IIF(COUNT(*) > 0, 1, 0) AttFlag
		FROM dbo.Attend 
		WHERE CONVERT(DATE, MeetingDate) BETWEEN @MeetingDay1 AND @MeetingDay2
		AND PeopleId = @pid AND OrganizationId = @oid
		GROUP BY dbo.SundayForDate(MeetingDate)
	), 
	codes AS (
		SELECT CASE WHEN a.AttFlag = 1 THEN 'P' ELSE '.' END pdot
		FROM dts d
		LEFT JOIN attends a ON a.MeetingDay = d.dt
	)
	SELECT @ret = (SELECT pdot AS [text()] FROM codes FOR XML PATH ('')) 

	RETURN @ret

END
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
