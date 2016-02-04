CREATE FUNCTION [dbo].[RecentAttendance] (@oid int)
RETURNS TABLE 
AS RETURN
(
	WITH attends AS (
		SELECT aa.PeopleId, MAX(m.MeetingDate) dt
		FROM dbo.Attend aa
		JOIN dbo.Meetings m ON aa.MeetingId = m.MeetingId
		JOIN dbo.Organizations o ON m.OrganizationId = o.OrganizationId
		WHERE m.OrganizationId = @oid
		AND (o.FirstMeetingDate IS NULL OR m.MeetingDate >= o.FirstMeetingDate)
		AND aa.AttendanceFlag = 1
		GROUP BY aa.PeopleId
	)
	SELECT a.PeopleId, 
		p.NAME,
		a.MeetingDate lastattend, 
		om.AttendPct,
		om.AttendStr,
		tt.DESCRIPTION attendtype,
		CASE WHEN om.OrganizationId = @oid THEN 0 ELSE 1 END visitor
	FROM attends AT
	JOIN Attend a ON AT.PeopleId = a.PeopleId AND a.OrganizationId = @oid AND a.MeetingDate = AT.dt
	JOIN lookup.AttendType tt ON a.AttendanceTypeId = tt.Id
	JOIN dbo.People p ON p.PeopleId = a.PeopleId
	LEFT JOIN dbo.OrganizationMembers om ON AT.PeopleId = om.PeopleId AND @oid = om.OrganizationId
	WHERE p.IsDeceased = 0
)



GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
