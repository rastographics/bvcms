
CREATE VIEW [dbo].[FirstAttends] AS 

	WITH firstmeets AS (
		SELECT 
			a.PeopleId,
			p.Name2,
			FirstAttend = MIN(a.MeetingDate)
		FROM dbo.Attend a
		JOIN dbo.People p ON p.PeopleId = a.PeopleId
		WHERE a.AttendanceFlag = 1
		GROUP BY a.PeopleId, p.Name2
	)
	SELECT fm.PeopleId,
		fm.Name2,
		aa.OrganizationId,
		o.OrganizationName,
		FirstAttend = FORMAT( fm.FirstAttend, 'M/d/yy')
	FROM firstmeets fm
	JOIN dbo.Attend aa ON aa.PeopleId = fm.PeopleId AND aa.MeetingDate = fm.FirstAttend
	JOIN dbo.Organizations o ON o.OrganizationId = aa.OrganizationId
	JOIN dbo.People p ON p.PeopleId = aa.PeopleId

GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
