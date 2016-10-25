CREATE FUNCTION [dbo].[AttendedAsOf]( 
	@progid INT,
	@divid INT,
	@org INT,
	@dt1 DATETIME, 
	@dt2 DATETIME,
	@guestonly BIT
	)
RETURNS TABLE
AS
RETURN
(
	SELECT DISTINCT
		a.PeopleId
		FROM dbo.Attend a
			JOIN dbo.Meetings m ON a.MeetingId = m.MeetingId
			JOIN dbo.Organizations o ON m.OrganizationId = o.OrganizationId
			WHERE AttendanceFlag = 1
			AND (ISNULL(@guestonly, 0) = 0 OR AttendanceTypeId IN (50, 60))
			AND a.MeetingDate >= @dt1
			AND a.MeetingDate < @dt2
			AND (ISNULL(@org, 0) = 0 OR m.OrganizationId = @org)
			AND (ISNULL(@divid, 0) = 0 
					OR EXISTS(SELECT NULL FROM dbo.DivOrg WHERE OrgId = m.OrganizationId AND DivId = @divid))
			AND (ISNULL(@progid, 0) = 0 
					OR EXISTS(SELECT NULL FROM dbo.DivOrg dd WHERE dd.OrgId = m.OrganizationId
						AND EXISTS(SELECT NULL FROM dbo.ProgDiv pp WHERE pp.DivId = dd.DivId AND pp.ProgId = @progid)))
)

GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
