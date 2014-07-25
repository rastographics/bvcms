CREATE FUNCTION [dbo].[LastMeetings](@orgid INT, @divid INT, @days INT)
RETURNS TABLE 
AS
RETURN 
(
		SELECT o.OrganizationId, o.OrganizationName, o.LeaderName, m.MeetingDate lastmeeting, m.MeetingId, m.Location
		FROM dbo.Organizations o
		JOIN dbo.Meetings m ON o.OrganizationId = m.OrganizationId 
			AND m.MeetingDate = ( SELECT MAX(a.MeetingDate) FROM dbo.Attend a
								  WHERE a.OrganizationId = o.OrganizationId
								  AND a.EffAttendFlag = 1 
								  AND a.MeetingDate > DATEADD(d, -@days, GETDATE()) 
								  AND a.MeetingDate < GETDATE()
								)
		WHERE (o.OrganizationId = @orgid OR @orgid IS NULL)
		AND (@divid IS NULL OR EXISTS(SELECT NULL FROM dbo.DivOrg WHERE OrgId = o.OrganizationId AND DivId = @divid))
		AND m.MeetingDate IS NOT NULL
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
