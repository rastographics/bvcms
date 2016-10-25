

CREATE VIEW [dbo].[AttendCredits]
AS
	SELECT
		OrganizationId,
		PeopleId,
		CONVERT(BIT, MAX(Attended)) AS Attended,
		DATEADD(ww, [Week], DATEADD(yy, [Year]-1900, 0)) - 4 - DATEPART(dw, DATEADD(ww, [Week], DATEADD(yy, [Year]-1900, 0)) - 4) + 1 WeekDate,
		OtherAttends = SUM(InlineView.OtherAttends)
	FROM (
		SELECT	a.OrganizationId,
				PeopleId,
				CONVERT(INT, EffAttendFlag) AS Attended, 
				DATEPART(yy, m.MeetingDate) AS [Year], 
				DATEPART(ww, m.MeetingDate) AS [Week], 
				s.ScheduleId,
				CASE WHEN ISNULL(m.AttendCreditId, 1) = 1 
					THEN AttendId + 20 -- make every meeting count, 20 gets it out of the way of AttendCredit codes
					ELSE m.AttendCreditId
				END AS AttendCredit,
				a.OtherAttends
		FROM dbo.Attend AS a
		JOIN dbo.Meetings m ON a.MeetingId = m.MeetingId
		LEFT JOIN dbo.OrgSchedule s 
			ON m.OrganizationId = s.OrganizationId 
			AND s.ScheduleId = dbo.ScheduleId(NULL, a.MeetingDate)
		WHERE m.MeetingDate >= DATEADD(yy, -3, (SELECT MAX(MeetingDate) FROM Attend WHERE AttendanceFlag = 1 AND OrganizationId = m.OrganizationId))
			AND m.MeetingDate <= DATEADD(hh ,ISNULL((select CONVERT(INT, Setting) FROM dbo.Setting WHERE Id = 'TZOffset'),0) + 10 /* to include future meetings for checkin */, GETDATE())
	) AS InlineView
	GROUP BY OrganizationId, PeopleId, [Year], [Week], AttendCredit
	



GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
