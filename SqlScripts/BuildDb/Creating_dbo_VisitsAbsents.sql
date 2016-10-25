
CREATE FUNCTION [dbo].[VisitsAbsents](@meetingid INT)
RETURNS TABLE 
AS
RETURN 
(
	SELECT 
		a.PeopleId
		, Name2 Name
		, CASE WHEN a.AttendanceFlag = 0
			THEN (	
				SELECT Description 
				FROM lookup.MemberType
				WHERE Id = a.MemberTypeId
			) ELSE (
				SELECT Description 
				FROM lookup.AttendType
				WHERE Id = a.AttendanceTypeId
			)
			END [status]
		, MemberType = ( 
			SELECT Description 
			FROM lookup.MemberType
			WHERE Id = a.MemberTypeId
		)
		, a.AttendanceFlag
		, a.MemberTypeId
		, om.LastAttended
		, om.AttendPct
		, om.AttendStr
		, dbo.Birthday(a.PeopleId) Birthday
		, p.EmailAddress
		, p.HomePhone
		, p.CellPhone
		, p.PrimaryAddress
		, p.PrimaryCity
		, p.PrimaryState
		, p.PrimaryZip
	FROM dbo.Attend a
	JOIN dbo.People p ON p.PeopleId = a.PeopleId
	LEFT JOIN dbo.OrganizationMembers om ON om.PeopleId = p.PeopleId AND om.OrganizationId = a.OrganizationId

	WHERE MeetingId = @meetingid
	AND (
		(a.AttendanceFlag = 1 AND a.AttendanceTypeId IN (40,50,60))
	  OR(a.AttendanceFlag = 0 AND a.MemberTypeId <> 310)
	)
	AND ISNULL(om.MemberTypeId, a.MemberTypeId) NOT IN (230, 311)
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
