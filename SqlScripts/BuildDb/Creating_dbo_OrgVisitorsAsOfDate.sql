CREATE FUNCTION [dbo].[OrgVisitorsAsOfDate](@orgid INT, @meetingdt DATETIME, @NoCurrentMembers BIT)

RETURNS TABLE 
AS
RETURN 
(
SELECT 
	CASE WHEN p.MemberStatusId = 10 THEN 'VM' ELSE 'VS' END VisitorType,
	p.PeopleId,
	p.FamilyId,
	p.PreferredName,
	p.LastName,
	p.BirthYear,
	p.BirthMonth,
	p.BirthDay,
	p.PrimaryAddress,
	p.PrimaryAddress2,
	p.PrimaryCity,
	p.PrimaryState,
	p.PrimaryZip,
	p.HomePhone,
	p.CellPhone,
	p.WorkPhone,
	ms.Description MemberStatus,
	p.EmailAddress Email,
	pp.Name2 BFTeacher,
	pp.PeopleId BFTeacherId,
	p.Age,
	dbo.LastAttended(@orgid, p.PeopleId) LastAttended
FROM dbo.People p
JOIN lookup.MemberStatus ms ON p.MemberStatusId = ms.Id
JOIN dbo.Families f ON f.FamilyId = p.FamilyId
LEFT JOIN dbo.Organizations om ON p.BibleFellowshipClassId = om.OrganizationId
LEFT JOIN dbo.People pp ON om.LeaderId = pp.PeopleId
WHERE EXISTS(SELECT NULL FROM dbo.Attend a 
			 JOIN dbo.Meetings m ON a.MeetingId = m.MeetingId
			 JOIN dbo.Organizations o ON m.OrganizationId = o.OrganizationId
			 WHERE a.PeopleId = p.PeopleId AND m.OrganizationId = @orgid
			 AND (a.AttendanceFlag = 1 OR a.Commitment IN (1, 4))
			 AND a.MeetingDate >= DATEADD(DAY, ISNULL(o.RollSheetVisitorWks, 3) * -7, @meetingdt)
			 AND a.MeetingDate <= @meetingdt
			 AND (a.MeetingDate > o.FirstMeetingDate OR o.FirstMeetingDate IS NULL)
			 AND a.AttendanceTypeId IN (40,50,60,110)
		)
AND EXISTS(SELECT NULL FROM dbo.OrgPeopleGuests(@orgid, 0) WHERE PeopleId = p.PeopleId)
AND  (@NoCurrentMembers = 0 
		OR NOT EXISTS(
			SELECT NULL FROM dbo.OrganizationMembers om 
			WHERE om.OrganizationId = @orgid 
			AND om.PeopleId = p.PeopleId
			AND om.MemberTypeId <> 311 -- prospect
		)
	)
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
