CREATE FUNCTION [dbo].[LastMeetings](@orgs VARCHAR(MAX))
RETURNS TABLE 
AS
RETURN 
(
		SELECT o.OrganizationId, o.OrganizationName, o.LeaderName, m.MeetingDate lastmeeting, m.MeetingId, m.Location
		FROM dbo.Organizations o
        JOIN SplitInts(@orgs) i ON i.Value = o.OrganizationId
		JOIN dbo.Meetings m ON o.OrganizationId = m.OrganizationId 
		WHERE m.MeetingDate = ( SELECT MAX(a.MeetingDate) FROM dbo.Attend a
								  WHERE a.OrganizationId = o.OrganizationId
								  AND a.EffAttendFlag = 1 
								  AND a.MeetingDate < GETDATE()
								)
		AND m.MeetingDate IS NOT NULL
)
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
