CREATE FUNCTION [dbo].[AttendCommitments](@oid INT)
RETURNS TABLE
AS RETURN
(
	SELECT 
		a.MeetingId,
		a.MeetingDate,
		a.PeopleId,
		p.Name2,
		a.Commitment,
		conflicts = CAST((CASE WHEN EXISTS(SELECT NULL FROM dbo.MeetingConflicts mc
							WHERE mc.PeopleId = a.PeopleId
							AND @oid IN (mc.OrgId1, mc.OrgId2))
						  THEN 1 ELSE 0 END) AS BIT)
	FROM dbo.Attend a
	JOIN dbo.People p ON p.PeopleId = a.PeopleId
	WHERE a.MeetingDate > GETDATE()
	AND a.OrganizationId = @oid
	AND a.Commitment IS NOT NULL
)
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
