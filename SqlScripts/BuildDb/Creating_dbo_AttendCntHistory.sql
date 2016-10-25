

CREATE FUNCTION [dbo].[AttendCntHistory]( 
	@progid INT,
	@divid INT,
	@org INT,
	@sched INT,
	@start DATETIME,
	@end DATETIME
	)
RETURNS TABLE
AS
RETURN
(
	SELECT 
		p.PeopleId, 
		(SELECT COUNT(*)
			FROM dbo.Attend a
			JOIN dbo.Meetings m ON a.MeetingId = m.MeetingId
			JOIN dbo.Organizations o ON m.OrganizationId = o.OrganizationId
			WHERE 1 = 1
			AND  AttendanceFlag = 1
			AND a.PeopleId = p.PeopleId
			AND a.MeetingDate >= @start
			AND a.MeetingDate < @end
			AND (@sched = 0
				OR (
					CAST(a.MeetingDate AS TIME) = CAST(CAST((@sched % 10000) / 100 AS VARCHAR) + ':' + CAST((@sched % 10000) % 100 AS VARCHAR) AS TIME) 
					AND ((@sched / 10000) = 11 OR DATEPART(weekday, a.MeetingDate) = (@sched / 10000))
				))
			AND (ISNULL(@org, 0) = 0 OR m.OrganizationId = @org)
			AND (ISNULL(@divid, 0) = 0 
					OR EXISTS(SELECT NULL FROM dbo.DivOrg WHERE OrgId = m.OrganizationId AND DivId = @divid))
			AND (ISNULL(@progid, 0) = 0 
					OR EXISTS(SELECT NULL FROM dbo.DivOrg dd WHERE dd.OrgId = m.OrganizationId
						AND EXISTS(SELECT NULL FROM dbo.ProgDiv pp WHERE pp.DivId = dd.DivId AND pp.ProgId = @progid)))
		) Cnt
	FROM dbo.People p
)


GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
