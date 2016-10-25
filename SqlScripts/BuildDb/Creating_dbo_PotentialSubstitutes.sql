CREATE FUNCTION [dbo].[PotentialSubstitutes] (@oid INT, @mid INT)
RETURNS TABLE 
AS
RETURN 
(
	SELECT 
		om.PeopleId
		, p.Name2
		, p.EmailAddress
		, (CASE WHEN (SELECT COUNT(*) FROM dbo.Attend a
						WHERE a.OrganizationId = @oid
						AND a.PeopleId = om.PeopleId
						AND a.MeetingDate > GETDATE()
						AND DATEPART(WEEKDAY, a.MeetingDate) = DATEPART(WEEKDAY, m.MeetingDate)
						AND DATEPART(HOUR, a.MeetingDate) = DATEPART(HOUR, m.MeetingDate)
						AND DATEPART(MINUTE, a.MeetingDate) = DATEPART(MINUTE, m.MeetingDate)
						AND a.Commitment IS NOT NULL) > 1 
				THEN 'same-schedule'
				END
		  ) SameSchedule
		, (CASE WHEN EXISTS (
					SELECT NULL FROM dbo.Attend a 
					WHERE a.MeetingId = @mid 
					AND a.PeopleId = om.PeopleId
					AND a.Commitment IS NOT NULL) 
				THEN 'committed'
			END
		  ) [Committed]
		, (SELECT STUFF((
				SELECT ' sg-' + CONVERT(VARCHAR, mt.Id)
				FROM dbo.OrgMemMemTags omt 
				JOIN dbo.MemberTags mt ON mt.Id = omt.MemberTagId 
				WHERE omt.OrgId = om.OrganizationId AND omt.PeopleId = om.PeopleId
				AND mt.Name LIKE 'SG:%'
				FOR XML PATH(''),TYPE
				).value('text()[1]','nvarchar(max)'),1,0,N''
		  )) Groups

	FROM dbo.OrganizationMembers om
	JOIN dbo.People p ON p.PeopleId = om.PeopleId
	JOIN Meetings m ON m.MeetingId = @mid
	WHERE om.OrganizationId = @oid
	AND om.MemberTypeId NOT IN (230, 311)
)

GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
