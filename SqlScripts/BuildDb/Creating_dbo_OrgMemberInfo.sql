CREATE FUNCTION [dbo].[OrgMemberInfo] (@oid INT)
RETURNS TABLE 
AS
RETURN 
(
SELECT i.*
	,LastAttendDt = m.MeetingDate
	,ContactReceived = ce.ContactDate
	,ContactMade = co.ContactDate
	,TaskAboutDt = ta.CreatedOn
	,TaskDelegatedDt = td.CreatedOn 
FROM 
(
	SELECT om.PeopleId
		,LastMeetingId = MAX(a.MeetingId)
		,ContacteeId = MAX(ce.ContactId)
		,ContactorId = MAX(co.ContactId)
		,TaskAboutId = MAX(ta.Id)
		,TaskDelegatedId = MAX(td.Id)
	FROM dbo.OrganizationMembers om
	JOIN dbo.People p ON p.PeopleId = om.PeopleId
	LEFT JOIN Attend a ON a.OrganizationId = @oid AND a.PeopleId = om.PeopleId AND a.AttendanceFlag = 1
	LEFT JOIN dbo.Contactees ce ON ce.PeopleId = om.PeopleId
	LEFT JOIN dbo.Contactors co ON co.PeopleId = om.PeopleId
	LEFT JOIN dbo.Task ta ON ta.WhoId = om.PeopleId AND ta.StatusId IN (10, 20) -- Active, Waiting
	LEFT JOIN dbo.Task td ON td.WhoId = om.PeopleId AND td.StatusId IN (10, 20) -- Active, Waiting
	WHERE om.OrganizationId = @oid 
	GROUP BY om.PeopleId, a.PeopleId, ce.PeopleId, co.PeopleId, ta.WhoId, td.CoOwnerId
) i
LEFT JOIN dbo.Meetings m ON m.MeetingId = i.LastMeetingId
LEFT JOIN Contact ce ON ce.ContactId = i.ContacteeId
LEFT JOIN contact co ON co.ContactId = i.ContactorId
LEFT JOIN Task ta ON ta.Id = i.TaskAboutId
LEFT JOIN Task td ON td.Id = i.TaskDelegatedId
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
