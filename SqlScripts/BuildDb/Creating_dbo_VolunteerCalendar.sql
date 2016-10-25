CREATE FUNCTION [dbo].[VolunteerCalendar] (@oid INT, @sg1 NVARCHAR(50), @sg2 NVARCHAR(50))
RETURNS TABLE 
AS
RETURN 
(
	SELECT om.PeopleId, 
		   p.Name2 AS Name,
	       commits = CAST(CASE WHEN EXISTS(SELECT NULL FROM dbo.Attend a
				                WHERE a.MeetingDate > GETDATE()
				                AND a.OrganizationId = om.OrganizationId
				                AND a.PeopleId = om.PeopleId
				                AND a.Commitment IN (1,4))
						THEN 1 ELSE 0 END AS BIT),
		   conflicts = CAST(CASE WHEN EXISTS(
								SELECT NULL FROM dbo.MeetingConflicts mc
								WHERE PeopleId = om.PeopleId 
								AND (mc.OrgId1 = om.OrganizationId OR mc.OrgId2 = om.OrganizationId))
						THEN 1 ELSE 0 END AS BIT)
	FROM dbo.OrganizationMembers om
	JOIN dbo.People p ON p.PeopleId = om.PeopleId
	WHERE om.OrganizationId = @oid
	AND (@sg1 IS NULL OR @sg1 = '(not specified)'
		OR EXISTS(SELECT NULL 
			FROM dbo.OrgMemMemTags omm
			JOIN dbo.MemberTags mt ON mt.Id = omm.MemberTagId
			WHERE mt.OrgId = om.OrganizationId 
			AND omm.PeopleId = om.PeopleId
			AND @sg1 = mt.Name))
	AND (@sg2 IS NULL OR @sg2 = '(not specified)'
		OR EXISTS(SELECT NULL 
			FROM dbo.OrgMemMemTags omm
			JOIN dbo.MemberTags mt ON mt.Id = omm.MemberTagId
			WHERE mt.OrgId = om.OrganizationId 
			AND omm.PeopleId = om.PeopleId
			AND @sg2 = mt.Name))
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
