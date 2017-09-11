CREATE VIEW [dbo].[LastAttends] AS 

	WITH lastmeets AS (
		SELECT 
			a.PeopleId,
			p.Name2,
			LastAttend = MAX(a.MeetingDate)
		FROM dbo.Attend a
		JOIN dbo.People p ON p.PeopleId = a.PeopleId
		WHERE a.AttendanceFlag = 1
		AND a.MeetingDate < DATEADD(DAY, 1, GETDATE())
		GROUP BY a.PeopleId, p.Name2
	)
	SELECT lm.PeopleId,
		lm.Name2,
		aa.OrganizationId,
		o.OrganizationName,
		LastAttend = FORMAT( lm.LastAttend, 'M/d/yy h:mm tt'),
		HomePhone = dbo.FmtPhone(p.HomePhone),
		CellPhone = dbo.FmtPhone(p.CellPhone),
		p.EmailAddress,
		HasHomePhone = CAST(CASE WHEN LEN(p.HomePhone) > 0 THEN 1 ELSE 0 END AS BIT),
		HasCellPhone = CAST(CASE WHEN LEN(p.CellPhone) > 0 THEN 1 ELSE 0 END AS BIT),
		HasEmail = CAST(CASE WHEN LEN(p.EmailAddress) > 0 THEN 1 ELSE 0 END AS BIT)
	FROM lastmeets lm
	JOIN dbo.Attend aa ON aa.PeopleId = lm.PeopleId AND aa.MeetingDate = lm.LastAttend
	JOIN dbo.Organizations o ON o.OrganizationId = aa.OrganizationId
	JOIN dbo.People p ON p.PeopleId = aa.PeopleId
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
